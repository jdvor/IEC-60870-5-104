namespace Iec608705104
{
    using System.Threading;

    public sealed class EstablishedConnectionStats
    {
        private long totalBytesRead;
        private int totalFramesRead;
        private int totalErrors;
        private int totalFrameReadErrors;

        public long BytesRead { get => totalBytesRead; }

        public int FramesRead { get => totalFramesRead; }

        public int Errors { get => totalErrors; }

        public int FrameReadErrors { get => totalFrameReadErrors; }

        internal long IncrementBytesRead(int bytes)
            => Interlocked.Add(ref totalBytesRead, bytes);

        internal int IncrementFramesRead()
            => Interlocked.Increment(ref totalFramesRead);

        internal int IncrementErrors()
            => Interlocked.Increment(ref totalErrors);

        internal int IncrementFrameReadErrors()
            => Interlocked.Increment(ref totalFrameReadErrors);

        public override string ToString()
            => $"{{ bytes: {BytesRead}, frames: {FramesRead}, errors: {Errors}, fre: {FrameReadErrors} }}";
    }
}
