using System;
using System.Collections.Generic;

namespace Iec608705104.Messages
{
    public sealed class FrameS : IFrame, IConfirmRecieval
    {
        /// <summary>
        /// Receive sequence number N(R)
        /// </summary>
        public ushort RecieveSeqNo { get; set; }

        public int ApduLength => Constants.AcpiLength;

        public FrameType Type { get; } = FrameType.S;

        public FrameS(ushort recieveSeqNo)
        {
            RecieveSeqNo = recieveSeqNo;
        }

        public int TryWrite(Span<byte> buffer)
        {
            buffer[0] = Constants.StartByte;
            buffer[1] = Constants.AcpiLength;
            buffer[2] = 0x02;
            buffer[3] = 0x00;
            var (lsb, msb) = Bytes.GetBytesFromN(RecieveSeqNo);
            buffer[4] = lsb;
            buffer[5] = msb;
            return Constants.MinFrameLength;
        }

        public static (ErrorCode, FrameS) Read(ReadOnlySpan<byte> buffer)
        {
            var recieveSeqNo = (ushort)Bytes.GetNFromBytes(buffer[2], buffer[3]);
            return (ErrorCode.None, new FrameS(recieveSeqNo));
        }

        public static readonly Dictionary<int, string> Annotations = new Dictionary<int, string>
        {
            { 0, "start (0x68)" },
            { 1, "APDU length (0x04)" },
            { 2, "S-Frame (00000010)" },
            { 3, "empty" },
            { 4, "N(R) LSB | 0" },
            { 5, "N(R) MSB" },
        };
    }
}
