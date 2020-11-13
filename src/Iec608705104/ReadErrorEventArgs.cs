namespace Iec608705104
{
    using System;

    public sealed class ReadErrorEventArgs : EventArgs
    {
        public EstablishedConnectionInfo ConnectionInfo { get; }

        public ErrorCode Error { get; }

        public Exception Exception { get; }

        public ReadErrorEventArgs(EstablishedConnectionInfo connectionInfo, ErrorCode error, Exception ex)
        {
            ConnectionInfo = connectionInfo;
            Error = error;
            Exception = ex;
        }
    }
}
