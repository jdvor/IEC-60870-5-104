namespace Iec608705104
{
    using Iec608705104.Messages;
    using Iec608705104.Messages.Asdu;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/
    /// </remarks>
    public sealed class ControlledStation : IControlledStation, IHostedService, IDisposable
    {
        private readonly ControlledStationOptions options;
        private readonly CancellationTokenSource cts;
        private readonly Dictionary<Guid, EstablishedConnection> connections;
        private Socket listenSocket;
        private Task acceptConnectionsTask;
        private bool disposed;

        public event EventHandler<ReadErrorEventArgs> ReadError;

        public event EventHandler<FrameReadEventArgs> FrameRead;

        public event EventHandler<DisconnectEventArgs> Disconnected;

        public ControlledStation(ControlledStationOptions options)
        {
            this.options = options;
            cts = new CancellationTokenSource();
            connections = new Dictionary<Guid, EstablishedConnection>(options.MaxParallelControllers);
        }

        public ControlledStation()
            : this(ControlledStationOptions.Default)
        {
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            cts.Dispose();
            listenSocket?.Dispose();
            disposed = true;
        }

        public void Send<T>(Guid connectionId, FrameI<T> frame)
            where T : IAsduValue, new()
            => SendImpl(connectionId, frame);

        public void Send(Guid connectionId, FrameU frame)
            => SendImpl(connectionId, frame);

        public void Send(Guid connectionId, FrameS frame)
            => SendImpl(connectionId, frame);

        private void SendImpl(Guid connectionId, IFrame frame)
        {
            var ok = connections.TryGetValue(connectionId, out var conn);
            if (!ok)
            {
                // ? exception
                return;
            }

            conn.Send(frame);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var endpoint = GetEndpoint(options);
            listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endpoint);
            listenSocket.Listen(options.MaxSocketBacklogSize);

            acceptConnectionsTask = AcceptConnectionsAsync();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cts.Cancel();
            listenSocket.ShutdownAndClose(options.SocketCloseTimeoutSecs);

            await acceptConnectionsTask.ConfigureAwait(false);

            var closingTasks = connections.Values.Select(x => x.CloseAsync());
            await Task.WhenAll(closingTasks).ConfigureAwait(false);
        }

        public bool TryFindConnection(string remoteHost, int remotePort, out EstablishedConnectionInfo eci)
        {
            var conn = connections.Values.FirstOrDefault(x => x.Info.RemoteHost == remoteHost && x.Info.RemotePort == remotePort);
            if (conn != null)
            {
                eci = conn.Info;
                return true;
            }

            eci = EstablishedConnectionInfo.Empty;
            return false;
        }

        private static IPEndPoint GetEndpoint(ControlledStationOptions options)
        {
            IPAddress ip;
            if (string.IsNullOrEmpty(options.Host))
            {
                ip = IPAddress.Loopback;
            }
            else
            {
                var host = Dns.GetHostEntry(options.Host);
                ip = host.AddressList[0];
            }

            return new IPEndPoint(ip, options.Port);
        }

        private async Task AcceptConnectionsAsync()
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    var acceptSocket = await listenSocket.AcceptAsync().ConfigureAwait(false);
                    if (acceptSocket != null)
                    {
                        var id = Guid.NewGuid();
                        var remote = (IPEndPoint)acceptSocket.RemoteEndPoint;
                        var eci = new EstablishedConnectionInfo(id, options.Host, options.Port, remote.Address.ToString(), remote.Port);
                        var clock = new AdjustedClock();
                        var conn = new EstablishedConnection(eci, acceptSocket, OnDisconnect, OnReadError, OnFrameRead, options, clock);
                        connections.Add(id, conn);
                    }
                }
                catch (Exception ex)
                {
                    OnReadError(EstablishedConnectionInfo.Empty, ErrorCode.AcceptSocket, ex);
                }
            }
        }

        private void OnReadError(EstablishedConnectionInfo info, ErrorCode error, Exception ex)
        {
            var h = ReadError;
            if (h != null)
            {
                h.Invoke(this, new ReadErrorEventArgs(info, error, ex));
            }
        }

        private void OnFrameRead(EstablishedConnectionInfo info, IFrame frame)
        {
            var h = FrameRead;
            if (h != null)
            {
                h.Invoke(this, new FrameReadEventArgs(info, frame));
            }
        }

        private void OnDisconnect(EstablishedConnectionInfo info)
        {
            connections.Remove(info.Id);
            var h = Disconnected;
            if (h != null)
            {
                h.Invoke(this, new DisconnectEventArgs(info));
            }
        }
    }
}
