namespace Iec608705104
{
    using Iec608705104.Messages;
    using System;

    public sealed class FrameReadEventArgs : EventArgs
    {
        public EstablishedConnectionInfo ConnectionInfo { get; }

        public IFrame Frame { get; }

        public FrameReadEventArgs(EstablishedConnectionInfo connectionInfo, IFrame frame)
        {
            ConnectionInfo = connectionInfo;
            Frame = frame;
        }
    }
}
