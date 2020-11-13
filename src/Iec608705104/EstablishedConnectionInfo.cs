namespace Iec608705104
{
    using System;

    public readonly struct EstablishedConnectionInfo : IEquatable<EstablishedConnectionInfo>
    {
        public static readonly EstablishedConnectionInfo Empty = new EstablishedConnectionInfo(0);

        public Guid Id { get; }

        public string LocalHost { get; }

        public int LocalPort { get; }

        public string RemoteHost { get; }

        public int RemotePort { get; }

        public bool IsEmpty => LocalPort == 0;

        public EstablishedConnectionInfo(Guid id, string localHost, int localPort, string remoteHost, int remotePort)
        {
            Id = id;
            LocalHost = localHost;
            LocalPort = localPort;
            RemoteHost = remoteHost;
            RemotePort = remotePort;
        }

        private EstablishedConnectionInfo(int x)
        {
            Id = default;
            LocalHost = string.Empty;
            LocalPort = x;
            RemoteHost = string.Empty;
            RemotePort = x;
        }

        public bool Equals(EstablishedConnectionInfo other)
            => Id == other.Id
            && LocalHost == other.LocalHost
            && LocalPort == other.LocalPort
            && RemoteHost == other.RemoteHost
            && RemotePort == other.RemotePort;

        public override bool Equals(object obj)
            => obj != null && obj is EstablishedConnectionInfo eci && Equals(eci);

        public override int GetHashCode()
            => Id.GetHashCode();

        public override string ToString()
            => $"{Id} {LocalHost}:{LocalPort} <-> {RemoteHost}:{RemotePort}";

        public static bool operator ==(EstablishedConnectionInfo left, EstablishedConnectionInfo right)
            => left.Equals(right);

        public static bool operator !=(EstablishedConnectionInfo left, EstablishedConnectionInfo right)
            => !left.Equals(right);
    }
}
