namespace Iec608705104
{
    public sealed class ControlledStationOptions : EstablishedConnectionOptions
    {
        public static readonly ControlledStationOptions Default = new ControlledStationOptions();

        public string Host { get; set; }

        public int Port { get; set; } = 2404;

        public int MaxSocketBacklogSize { get; set; } = 100;

        public int MaxParallelControllers { get; set; } = 100;
    }
}
