namespace Iec608705104
{
    using System;

    public interface IClock
    {
        TimeSpan Adjustment { get; }

        DateTimeOffset Now { get; }

        TimeZoneInfo TimeZone { get; }

        void UpdateAdjustment(TimeSpan adjustment);
    }
}
