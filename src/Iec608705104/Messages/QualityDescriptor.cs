namespace Iec608705104.Messages
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public readonly struct QualityDescriptor : IEquatable<QualityDescriptor>
    {
        public bool Overflow { get; }

        public bool Blocked { get; }

        public bool Substituted { get; }

        public bool NonTopical { get; }

        public bool Invalid { get; }

        public QualityDescriptor(bool overflow, bool blocked, bool substituted, bool nonTopical, bool invalid)
        {
            Overflow = overflow;
            Blocked = blocked;
            Substituted = substituted;
            NonTopical = nonTopical;
            Invalid = invalid;
        }

        public override string ToString()
            => $"QDS (OV: {(Overflow ? 1 : 0)}, BL: {(Blocked ? 1 : 0)}, SB: {(Substituted ? 1 : 0)}, NT: {(NonTopical ? 1 : 0)}, IV: {(Invalid ? 1 : 0)})";

        public bool Equals(QualityDescriptor other)
        {
            return
                Overflow == other.Overflow &&
                Blocked == other.Blocked &&
                Substituted == other.Substituted &&
                NonTopical == other.NonTopical &&
                Invalid == other.Invalid;
        }

        public override bool Equals(object obj)
            => obj is QualityDescriptor qds && Equals(qds);

        public override int GetHashCode()
            => ((byte)this).GetHashCode();

        public static bool operator ==(QualityDescriptor left, QualityDescriptor right)
            => left.Equals(right);

        public static bool operator !=(QualityDescriptor left, QualityDescriptor right)
            => !left.Equals(right);

        public static explicit operator QualityDescriptor(byte b)
        {
            return new QualityDescriptor(
                (b & 0x01) != 0,
                (b & 0x10) != 0,
                (b & 0x20) != 0,
                (b & 0x40) != 0,
                (b & 0x80) != 0);
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503", Justification = "more readable easy code")]
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025", Justification = "more readable easy code")]
        public static explicit operator byte(QualityDescriptor qds)
        {
            // bits:
            // ------------------------------
            // 7   6   5   4   3   2   1   0
            // ------------------------------
            // IV  NT  SB  BL  -   -   -   OV
            // ------------------------------
            byte b = 0;
            if (qds.Overflow) b |= 0x01; else b &= 0xFE;
            if (qds.Blocked) b |= 0x10; else b &= 0xEF;
            if (qds.Substituted) b |= 0x20; else b &= 0xDF;
            if (qds.NonTopical) b |= 0x40; else b &= 0xBF;
            if (qds.Invalid) b |= 0x80; else b &= 0x7F;
            return b;
        }
    }
}
