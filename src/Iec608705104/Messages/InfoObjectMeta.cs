namespace Iec608705104.Messages
{
    using Iec608705104.Messages.Asdu;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public readonly struct InfoObjectMeta : IEquatable<InfoObjectMeta>
    {
        public static readonly InfoObjectMeta Empty = new InfoObjectMeta(0);

        public int EncodedLength { get; }

        public bool SupportsSequence { get; }

        public CauseOfTransmission[] AcceptableCauseOfTransmission { get; }

        public bool IsEmpty => EncodedLength == 0;

        public InfoObjectMeta(int encodedLength, bool supportsSequence, CauseOfTransmission[] acceptableCauseOfTransmission)
        {
            EncodedLength = encodedLength;
            SupportsSequence = supportsSequence;
            AcceptableCauseOfTransmission = acceptableCauseOfTransmission;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313", Justification = "parameter-less ctor is not allowed, so _ is justified")]
        private InfoObjectMeta(int _)
        {
            EncodedLength = 0;
            SupportsSequence = false;
            AcceptableCauseOfTransmission = null;
        }

        public bool Equals(InfoObjectMeta other)
        {
            if (EncodedLength != other.EncodedLength ||
                SupportsSequence == other.SupportsSequence ||
                (AcceptableCauseOfTransmission?.Length ?? 0) != (other.AcceptableCauseOfTransmission?.Length ?? 0))
            {
                return false;
            }

            if (AcceptableCauseOfTransmission != null && other.AcceptableCauseOfTransmission != null)
            {
                for (int i = 0; i < AcceptableCauseOfTransmission.Length; i++)
                {
                    if (AcceptableCauseOfTransmission[i] != other.AcceptableCauseOfTransmission[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
            => obj is InfoObjectMeta iom && Equals(iom);

        public override int GetHashCode()
        {
            const int k = 16777619;
            unchecked
            {
                int h = (int)2166136261;
                h = (h * k) ^ EncodedLength.GetHashCode();
                h = (h * k) ^ SupportsSequence.GetHashCode();
                h = (h * k) ^ AcceptableCauseOfTransmission?.Length.GetHashCode() ?? 0;
                return h;
            }
        }

        public static bool operator ==(InfoObjectMeta left, InfoObjectMeta right)
            => left.Equals(right);

        public static bool operator !=(InfoObjectMeta left, InfoObjectMeta right)
            => !left.Equals(right);

        public static InfoObjectMeta Get<T>()
            where T : IAsduValue, new()
        {
            return Empty;
        }
    }
}
