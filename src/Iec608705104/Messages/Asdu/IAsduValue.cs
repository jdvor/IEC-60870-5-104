namespace Iec608705104.Messages.Asdu
{
    using System;

    public interface IAsduValue
    {
        public int EncodedLength { get; }

        int TryWrite(Span<byte> buffer);

        ErrorCode TryRead(ReadOnlySpan<byte> buffer);

        InfoObjectMeta CreateMeta();
    }
}
