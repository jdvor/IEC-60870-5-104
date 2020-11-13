namespace Iec608705104
{
    using Iec608705104.Messages;
    using System;

    public interface IStation
    {
        event EventHandler<ReadErrorEventArgs> ReadError;

        event EventHandler<FrameReadEventArgs> FrameRead;

        event EventHandler<DisconnectEventArgs> Disconnected;

        void Send(Guid connectionId, FrameU frame);

        void Send(Guid connectionId, FrameS frame);
    }
}
