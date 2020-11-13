namespace Iec608705104
{
    using System;

    public sealed class DisconnectEventArgs : EventArgs
    {
        public EstablishedConnectionInfo ConnectionInfo { get; }

        public DisconnectEventArgs(EstablishedConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;
        }
    }
}
