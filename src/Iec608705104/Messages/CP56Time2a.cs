namespace Iec608705104.Messages
{
    using System;

    public readonly struct CP56Time2a : IEquatable<CP56Time2a>
    {
        public const int YearZero = 2000;
        public const int EncodedLength = 7;

        public static readonly CP56Time2a Empty = new CP56Time2a(0);

        public bool IsEmpty => Year == -1;

        public int Year { get; }

        public int Month { get; }

        public int Day { get; }

        public int Hour { get; }

        public int Minute { get; }

        public int Second { get; }

        public int Millisecond { get; }

        /// <summary>
        /// Indicates whether this <see cref="CP26Time2a"/> was substitued by an intermediate station
        /// </summary>
        /// <value><c>true</c> if substitued; otherwise, <c>false</c>.</value>
        public bool IsSubstitued { get; }

        public bool IsSummerTime { get; }

        public CP56Time2a(int year, int month, int day, int hour, int min, int sec, int ms, bool isSubstitued, bool isSummerTime)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = min;
            Second = sec;
            Millisecond = ms;
            IsSubstitued = isSubstitued;
            IsSummerTime = isSummerTime;
        }

        private CP56Time2a(int _)
        {
            Year = -1;
            Month = -1;
            Day = -1;
            Hour = -1;
            Minute = -1;
            Second = -1;
            Millisecond = -1;
            IsSubstitued = false;
            IsSummerTime = false;
        }

        public static CP56Time2a ReadBuffer(ReadOnlySpan<byte> buffer)
        {
#if !SKIP_CHECKS
            if (buffer.Length < EncodedLength)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), $"CP56Time2a requires 7 octets (bytes), which are not available; buffer length: {buffer.Length}");
            }
#endif

            var tms = buffer[0] + (buffer[1] * 0x100);
            var ms = tms % 1000;
            var sec = tms / 1000;
            var min = buffer[2] & 0x3F;
            var hour = buffer[3] & 0x1F;
            var day = buffer[4] & 0x1F;
            var month = buffer[5] & 0x0F;
            var year = (buffer[6] & 0x7F) + YearZero;
            var isSummerTime = (buffer[3] & 0x80) != 0;
            var isSubstitued = (buffer[2] & 0x40) == 0x40;

            return new CP56Time2a(year, month, day, hour, min, sec, ms, isSubstitued, isSummerTime);
        }

        public int Write(Span<byte> buffer)
        {
#if !SKIP_CHECKS
            if (buffer.Length < EncodedLength)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), $"CP56Time2a requires 7 octets (bytes), which are not available; buffer length: {buffer.Length}");
            }
#endif
            int tms = (1000 * Second) + Millisecond;
            buffer[0] = (byte)(tms & 0xFF);
            buffer[1] = (byte)((tms / 0x100) & 0xFF);
            buffer[2] = IsSubstitued
                ? (byte)(Minute | 0x40)
                : (byte)(Minute & 0xBF);
            buffer[3] = IsSummerTime
                ? (byte)(Hour | 0x80)
                : (byte)(Hour & 0x7F);
            buffer[4] = (byte)Day;
            buffer[5] = (byte)Month;
            buffer[6] = (byte)(Year - YearZero);

            return EncodedLength;
        }

        public bool Equals(CP56Time2a other) =>
            Year == other.Year
            && Month == other.Month
            && Day == other.Day
            && Hour == other.Hour
            && Minute == other.Minute
            && Second == other.Second
            && Millisecond == other.Millisecond
            && IsSubstitued == other.IsSubstitued
            && IsSummerTime == other.IsSummerTime;

        public override bool Equals(object obj)
            => obj != null && obj is CP56Time2a t && Equals(t);

        public override int GetHashCode()
        {
            const int k = 16777619;
            unchecked
            {
                int h = (int)2166136261;
                h = (h * k) ^ Year.GetHashCode();
                h = (h * k) ^ Month.GetHashCode();
                h = (h * k) ^ Day.GetHashCode();
                h = (h * k) ^ Hour.GetHashCode();
                h = (h * k) ^ Minute.GetHashCode();
                h = (h * k) ^ Second.GetHashCode();
                h = (h * k) ^ Millisecond.GetHashCode();
                h = (h * k) ^ IsSubstitued.GetHashCode();
                h = (h * k) ^ IsSummerTime.GetHashCode();
                return h;
            }
        }

        public override string ToString()
            => $"{Year:D4}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2}.{Millisecond:D3}";

        public static bool operator ==(CP56Time2a left, CP56Time2a right)
            => left.Equals(right);

        public static bool operator !=(CP56Time2a left, CP56Time2a right)
            => !left.Equals(right);
    }
}
