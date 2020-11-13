namespace Iec608705104
{
    using System;

    public sealed class AdjustedClock : IClock
    {
        private bool adjust;
        private readonly bool convert;

        public TimeSpan Adjustment { get; private set; }

        public TimeZoneInfo TimeZone { get; }

        public DateTimeOffset Now
        {
            get
            {
                var time = adjust
                    ? DateTimeOffset.Now
                    : DateTimeOffset.Now.Add(Adjustment);
                return convert
                    ? TimeZoneInfo.ConvertTime(time, TimeZone)
                    : time;
            }
        }

        public AdjustedClock(TimeSpan adjustment, TimeZoneInfo tz)
        {
            adjust = !adjustment.Equals(TimeSpan.Zero);
            convert = tz != null && tz.Id != TimeZoneInfo.Local.Id;
            Adjustment = adjustment;
            TimeZone = tz ?? TimeZoneInfo.Local;
        }

        public AdjustedClock()
            : this(TimeSpan.Zero, null)
        {
        }

        public void UpdateAdjustment(TimeSpan adjustment)
        {
            Adjustment = adjustment;
            adjust = !adjustment.Equals(TimeSpan.Zero);
        }
    }
}
