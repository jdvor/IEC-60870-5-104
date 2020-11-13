namespace Iec608705104
{
    using Iec608705104.Messages;
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.IO.Pipelines;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class EstablishedConnection : IEstablishedConnection
    {
        private readonly Socket socket;
        private readonly NetworkStream networkStream;
        private readonly Action<EstablishedConnectionInfo> onDisconnect;
        private readonly Action<EstablishedConnectionInfo, ErrorCode, Exception> onError;
        private readonly Action<EstablishedConnectionInfo, IFrame> onFrameRead;
        private readonly EstablishedConnectionOptions options;
        private readonly IClock clock;
        private readonly CancellationTokenSource cts;
        private readonly Pipe pipe;
        private readonly Task listenTask;
        private bool disposed;

        public Guid Id => Info.Id;

        public EstablishedConnectionInfo Info { get; }

        public EstablishedConnectionState State { get; private set; }

        public EstablishedConnectionStats Stats { get; }

        public EstablishedConnection(
            EstablishedConnectionInfo eci,
            Socket socket,
            Action<EstablishedConnectionInfo> onDisconnect,
            Action<EstablishedConnectionInfo, ErrorCode, Exception> onError,
            Action<EstablishedConnectionInfo, IFrame> onFrameRead,
            EstablishedConnectionOptions options,
            IClock clock)
        {
            Info = eci.IsEmpty ? throw new ArgumentException("Connection info must not be empty", nameof(eci)) : eci;
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.onDisconnect = onDisconnect ?? throw new ArgumentNullException(nameof(onDisconnect));
            this.onError = onError ?? throw new ArgumentNullException(nameof(onError));
            this.onFrameRead = onFrameRead ?? throw new ArgumentNullException(nameof(onFrameRead));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            networkStream = new NetworkStream(socket);
            cts = new CancellationTokenSource();
            pipe = new Pipe(new PipeOptions(minimumSegmentSize: options.SocketReadMinBufferSize));
            State = EstablishedConnectionState.Opened;
            Stats = new EstablishedConnectionStats();

            listenTask = ListenForIncomingFrames();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            cts.Dispose();
            networkStream.Dispose();
            socket.Dispose();

            disposed = true;
        }

        public async Task CloseAsync()
        {
            State = EstablishedConnectionState.Closing;

            cts.Cancel();
            pipe.Reader.CancelPendingRead();
            networkStream.Close();
            socket.ShutdownAndClose(options.SocketCloseTimeoutSecs);

            await listenTask.ConfigureAwait(false);

            State = EstablishedConnectionState.Closed;
        }

        public int Send(IFrame frame)
        {
            var span = pipe.Writer.GetSpan(frame.ApduLength);
            var bytesWritten = frame.TryWrite(span);
            networkStream.Write(span.Slice(0, bytesWritten));
            return bytesWritten;
        }

        private Task ListenForIncomingFrames()
        {
            Task writing = FillPipeAsync(socket, pipe.Writer);
            Task reading = ReadPipeAsync(pipe.Reader);
            return Task.WhenAll(reading, writing);
        }

        private async Task FillPipeAsync(Socket socket, PipeWriter writer)
        {
            try
            {
                while (State == EstablishedConnectionState.Opened)
                {
                    var memory = writer.GetMemory(options.SocketReadMinBufferSize);
                    var bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None, cts.Token).ConfigureAwait(false);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    writer.Advance(bytesRead);
                    Stats.IncrementBytesRead(bytesRead);

                    var result = await writer.FlushAsync(cts.Token).ConfigureAwait(false);
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                writer.Complete();
            }
            catch (Exception ex)
            {
                OnError(ErrorCode.FillPipe, ex);
                writer.Complete(ex);
            }
        }

        private async Task ReadPipeAsync(PipeReader reader)
        {
            try
            {
                while (State == EstablishedConnectionState.Opened)
                {
                    ReadResult result = await reader.ReadAsync(cts.Token).ConfigureAwait(false);

                    ReadOnlySequence<byte> buffer = result.Buffer;
                    if (buffer.Length == 0)
                    {
                        continue;
                    }

                    SequencePosition consumed;
                    SequencePosition examined;
                    try
                    {
                        (consumed, examined) = ReadFrames(ref buffer);
                        Debug.WriteLine($"ADVANCE_TO consumed: {consumed.GetInteger()}, examined: {examined.GetInteger()}");
                    }
                    catch (Exception ex)
                    {
                        OnError(ErrorCode.FailedToReadFrames, ex);
                        consumed = buffer.End;
                        examined = buffer.End;
                    }

                    reader.AdvanceTo(consumed, examined);
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                await reader.CompleteAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnError(ErrorCode.ReadPipe, ex);
                await reader.CompleteAsync(ex).ConfigureAwait(false);
            }
        }

        private (SequencePosition consumed, SequencePosition examined) ReadFrames(ref ReadOnlySequence<byte> buffer)
        {
            buffer.Debug($"BUFFER len: {buffer.Length}, start: {buffer.Start.GetInteger()}, end: {buffer.End.GetInteger()}");

            const int MinPossibleFrameLength = 6;
            if (buffer.Length < MinPossibleFrameLength)
            {
                Debug.WriteLine($"SMALL_BUFFER {buffer.Length}");
                return (buffer.Start, buffer.End);
            }

            var seq = new SequenceReader<byte>(buffer);
            byte apduLength = 0;
            var foundStart = seq.TryAdvanceTo(0x68, advancePastDelimiter: true) && seq.TryRead(out apduLength);
            if (!foundStart)
            {
                seq.Debug("NO_START_BYTE");
                return (seq.Position, seq.Position);
            }

            var validApduLength = apduLength == 4 || (apduLength >= Constants.NoPrefix.MinFrameILength && apduLength <= Constants.MaxApduLength);
            if (!validApduLength)
            {
                seq.Debug("INVALID_APDU", apduLength);
                OnError(ErrorCode.InvalidApduLength, null);
                return (seq.Position, seq.Position);
            }

            var isThereEnoughData = seq.Remaining >= apduLength;
            if (!isThereEnoughData)
            {
                seq.Debug("NOT_ENOUGH_DATA", apduLength);
                return (buffer.Start, seq.Position);
            }

            var success = TryCreateFrameSpan(in seq, apduLength, out var frameSpan);
            if (!success)
            {
                seq.Debug("FAILED_FRAME_SPAN");
                OnError(ErrorCode.FailedToCreateFrameSpan, null);
                return (seq.Position, seq.Position);
            }

            seq.Debug("FRAME_READ", apduLength);
            frameSpan.Debug("FRAME");

            var (error, frame) = Bytes.ReadFrame(frameSpan);
            if (error == ErrorCode.None)
            {
                OnFrameRead(frame);
            }
            else
            {
                Stats.IncrementFrameReadErrors();
                OnError(error, null);
            }

            var at = buffer.GetPosition(apduLength, seq.Position);
            return (at, at);
        }

        private bool TryCreateFrameSpan(in SequenceReader<byte> seq, byte apduLength, out ReadOnlySpan<byte> frameSpan)
        {
            var buffer = seq.Sequence;
            if (buffer.IsSingleSegment)
            {
                var firstSpan = buffer.First.Span;
                var spanStartInSeq = buffer.Start.GetInteger();
                var start = seq.Position.GetInteger() - spanStartInSeq;
                frameSpan = firstSpan.Slice(start, apduLength);
                return true;
            }

            var span = pipe.Writer.GetSpan(apduLength).Slice(0, apduLength);
            var success = seq.TryCopyTo(span);
            if (success)
            {
                frameSpan = span;
                return true;
            }

            frameSpan = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnError(ErrorCode code, Exception ex)
        {
            Stats.IncrementErrors();
            onError(Info, code, ex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnFrameRead(IFrame frame)
        {
            Stats.IncrementFramesRead();
            onFrameRead(Info, frame);
        }

        public override string ToString()
            => $"{State} {Info}";
    }
}
