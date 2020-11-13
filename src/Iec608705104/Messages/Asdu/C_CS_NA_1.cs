namespace Iec608705104.Messages.Asdu
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Time synchronization command
    /// </summary>
    [SuppressMessage("Naming", "CA1707", Justification = Constants.AsduValueNameJustification)]
    [SuppressMessage("Style", "IDE1006", Justification = Constants.AsduValueNameJustification)]
    public sealed class C_CS_NA_1 : IAsduValue
    {
        private static readonly CauseOfTransmission[] AcceptableCauseOfTransmission = new[]
        {
            CauseOfTransmission.ACTIVATION,
            CauseOfTransmission.SPONTANEOUS,
            CauseOfTransmission.UNKNOWN_TYPE_ID,
            CauseOfTransmission.UNKNOWN_CAUSE_OF_TRANSMISSION,
            CauseOfTransmission.UNKNOWN_COMMON_ADDRESS_OF_ASDU,
            CauseOfTransmission.UNKNOWN_INFORMATION_OBJECT_ADDRESS,
        };

        public CP56Time2a Value { get; private set; }

        public C_CS_NA_1()
        {
            Value = CP56Time2a.Empty;
        }

        public C_CS_NA_1(CP56Time2a value)
        {
            Value = !value.IsEmpty ? value : throw new ArgumentException("CP56Time2a must not be empty.", nameof(value));
        }

        public int EncodedLength => CP56Time2a.EncodedLength;

        public InfoObjectMeta CreateMeta()
        {
            return new InfoObjectMeta(
                encodedLength: EncodedLength,
                supportsSequence: false,
                acceptableCauseOfTransmission: AcceptableCauseOfTransmission);
        }

        public ErrorCode TryRead(ReadOnlySpan<byte> buffer)
        {
            try
            {
                Value = CP56Time2a.ReadBuffer(buffer);
                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.InvalidCP56Time2a;
            }
        }

        public int TryWrite(Span<byte> buffer)
        {
            if (Value != null)
            {
                try
                {
                    return Value.Write(buffer);
                }
                catch
                {
                }
            }

            return 0;
        }
    }
}
