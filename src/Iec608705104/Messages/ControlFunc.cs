namespace Iec608705104.Messages
{
    public enum ControlFunc : byte
    {
        UNKNOWN = 0x00,

        /// <summary>
        /// Test Frame Activation
        /// </summary>
        TESTFR_ACT = 0x43,

        /// <summary>
        /// Test Frame Confirmation
        /// </summary>
        TESTFR_CON = 0x83,

        /// <summary>
        /// Start Data Transfer Activation
        /// </summary>
        STARTDT_ACT = 0x13,

        /// <summary>
        /// Start Data Transfer Confirmation
        /// </summary>
        STARTDT_CON = 0x23,

        /// <summary>
        /// Stop Data Transfer Activation
        /// </summary>
        STOPDT_ACT = 0x07,

        /// <summary>
        /// Stop Data Transfer Confirmation
        /// </summary>
        STOPDT_CON = 0x0B,
    }
}
