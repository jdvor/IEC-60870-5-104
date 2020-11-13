namespace Iec608705104.Messages
{
    using System;

    public interface IFrame
    {
        FrameType Type { get; }

        int ApduLength { get; }

        int TryWrite(Span<byte> buffer);
    }
}
