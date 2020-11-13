using System;
using System.Collections.Generic;

namespace Iec608705104.Messages
{
    public sealed class FrameU : IFrame
    {
        public static readonly FrameU TestFrAct = new FrameU(ControlFunc.TESTFR_ACT);
        public static readonly FrameU TestFrCon = new FrameU(ControlFunc.TESTFR_CON);
        public static readonly FrameU StartDtAct = new FrameU(ControlFunc.STARTDT_ACT);
        public static readonly FrameU StartDtCon = new FrameU(ControlFunc.STARTDT_CON);
        public static readonly FrameU StopDtAct = new FrameU(ControlFunc.STOPDT_ACT);
        public static readonly FrameU StopDtCon = new FrameU(ControlFunc.STOPDT_CON);

        public ControlFunc ControlFunc { get; }

        public int ApduLength => Constants.AcpiLength;

        public FrameType Type { get; } = FrameType.U;

        public FrameU(ControlFunc controlFuncId)
        {
            ControlFunc = controlFuncId;
        }

        public int TryWrite(Span<byte> buffer)
        {
            buffer[0] = Constants.StartByte;
            buffer[1] = Constants.AcpiLength;
            buffer[2] = (byte)ControlFunc;
            buffer[3] = 0x00;
            buffer[4] = 0x00;
            buffer[5] = 0x00;
            return Constants.MinFrameLength;
        }

        public static (ErrorCode, FrameU) Read(ReadOnlySpan<byte> buffer)
        {
            return buffer[0] switch
            {
                (byte)ControlFunc.TESTFR_ACT => (ErrorCode.None, TestFrAct),
                (byte)ControlFunc.TESTFR_CON => (ErrorCode.None, TestFrCon),
                (byte)ControlFunc.STARTDT_ACT => (ErrorCode.None, StartDtAct),
                (byte)ControlFunc.STARTDT_CON => (ErrorCode.None, StartDtCon),
                (byte)ControlFunc.STOPDT_ACT => (ErrorCode.None, StopDtAct),
                (byte)ControlFunc.STOPDT_CON => (ErrorCode.None, StopDtCon),
                _ => (ErrorCode.UnknownControlFunc, null),
            };
        }

        public static readonly Dictionary<int, string> Annotations = new Dictionary<int, string>
        {
            { 0, "start (0x68)" },
            { 1, "APDU length (0x04)" },
            { 2, "TESTFR_ACT | TESTFR_CON | STARTDT_ACT | STARTDT_CON | STOPDT_ACT | STOPDT_CON | 1 | 1" },
            { 3, "empty" },
            { 4, "empty" },
            { 5, "empty" },
        };
    }
}
