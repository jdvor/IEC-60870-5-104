namespace Iec608705104.Messages
{
    public static class Constants
    {
        public const int PrefixLength = 2;

        public const int MaxApduLength = 255 - PrefixLength;

        public const byte StartByte = 0x68;

        public const int AcpiLength = 4;

        public const int MinFrameLength = PrefixLength + AcpiLength;

        public const int FrameIHeaderLength = PrefixLength + AcpiLength + 6;

        public const int FrameITypeIndex = 6;

        public const int MinFrameILength = FrameIHeaderLength + 1;

        /// <summary>
        /// Uses range 0 – 127.
        /// 0 means ASDU contains no information objects (IO).
        /// </summary>
        public const int MaxInfoObjects = 127;

        public const int InfoObjectAddressLength = 3;

        public const int NoIoaSequence = -1;

        public const string AsduValueNameJustification = "ASDU value type names with underscores is a domain convention within IEC 60870-5-101/4.";

        public static class NoPrefix
        {
            public const int MinFrameLength = Constants.MinFrameLength - PrefixLength;

            public const int FrameIHeaderLength = Constants.FrameIHeaderLength - PrefixLength;

            public const int FrameITypeIndex = Constants.FrameITypeIndex - PrefixLength;

            public const int MinFrameILength = Constants.MinFrameILength - PrefixLength;
        }
    }
}
