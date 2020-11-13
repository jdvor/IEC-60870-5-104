namespace Iec608705104
{
    using Iec608705104.Messages;
    using System;
    using System.Threading.Tasks;


    internal interface IEstablishedConnection : IDisposable
    {
        Guid Id { get; }

        EstablishedConnectionInfo Info { get; }

        EstablishedConnectionState State { get; }

        EstablishedConnectionStats Stats { get; }

        Task CloseAsync();

        int Send(IFrame frame);
    }
}
