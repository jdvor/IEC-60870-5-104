namespace Iec608705104
{
    using Iec608705104.Messages;
    using Iec608705104.Messages.Asdu;
    using System;

    public interface IControlledStation : IStation
    {
        void Send<T>(Guid connectionId,  FrameI<T> frame)
            where T : IAsduValue, new();
    }
}
