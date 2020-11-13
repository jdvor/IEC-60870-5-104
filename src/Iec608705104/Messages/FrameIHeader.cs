namespace Iec608705104.Messages
{
    using System;
    using System.Collections.Generic;

    public sealed class FrameIHeader
    {
        /// <summary>
        /// Send sequence number N(S)
        /// </summary>
        public ushort SendSeqNo { get; }

        /// <summary>
        /// Receive sequence number N(R)
        /// </summary>
        public ushort RecieveSeqNo { get; }

        public AsduTypeId AsduTypeId { get; }

        /// <summary>
        /// If <code>true</code> it means the individual information objects will not contain their own address,
        /// but rather their index is added to single information object address at the begining of the sequence.
        /// Otherwise (when <code>false</code>) each information object will contain its own independent address.
        /// See documentation table TBD.
        public bool IsSequence { get; }

        /// <summary>
        /// If <code>true</code> the message is not intended to control the process
        /// or change the system state; otherwise <code>false</code>.
        /// </summary>
        public bool IsTest { get; }

        /// <summary>
        /// (P/N) is meaningful when used with control commands.
        /// The bit is used when the control command is mirrored in the monitor direction,
        /// and it provides indication of whether the command was executed or not.
        /// When the PN bit is not relevant, it is set to zero.
        /// </summary>
        public bool IsNegativeConfirmation { get; }

        /// <summary>
        /// Cause of transmission (COT)
        /// COT is used to control the routing of messages both on the communication network,
        /// and within a station, directing by ASDU to the correct program or task for processing.
        /// ASDUs in control direction are confirmed application services
        /// and may be mirrored in monitor direction with different causes of transmission.
        /// 0 is not defined,
        /// 1-47 is used for standard definitions of this companion standard (compatible range), see Appendix C.2,
        /// 48-63 is for special use(private range).
        /// </summary>
        public CauseOfTransmission CauseOfTransmission { get; }

        /// <summary>
        /// Originator address (ORG)
        /// The originator address is optional on a system basis. It provides a means for a controlling station
        /// to explicitly identify itself. This is not necessary when there is
        /// only one controlling station in a system, but is required when there is more than
        /// one controlling station, or some stations are dual-mode stations. In this case the
        /// originator address can be used to direct command confirmations back to the
        /// particular controlling station rather than to the whole system.
        /// </summary>
        public byte OriginatorAddress { get; }

        /// <summary>
        /// Common address of the ASDU (CA)
        /// The address is called common address because it is associated with all objects
        /// contained within the ASDU.This is normally interpreted as a station address,
        /// however it can be structured to form a station/sector address where individual
        /// stations are broken up into multiple logical units.
        /// Value 0 is not used,
        /// range 1 – 65 534 means a station address,
        /// value 65 535 (0xFFFF) means global address.
        /// </summary>
        public ushort CommonAddress { get; }

        public int InfoObjectsCount { get; }

        public int ApduLength { get; }

        public FrameIHeader(
            ushort sendSeqNo,
            ushort recieveSeqNo,
            AsduTypeId asduTypeId,
            bool isSequence,
            bool isTest,
            bool isNegativeConfirmation,
            CauseOfTransmission causeOfTransmission,
            byte originatorAddress,
            ushort commonAddress,
            int infoObjectsCount,
            int apduLength)
        {
            SendSeqNo = sendSeqNo;
            RecieveSeqNo = recieveSeqNo;
            AsduTypeId = asduTypeId;
            IsSequence = isSequence;
            IsTest = isTest;
            IsNegativeConfirmation = isNegativeConfirmation;
            CauseOfTransmission = causeOfTransmission;
            OriginatorAddress = originatorAddress;
            CommonAddress = commonAddress;
            InfoObjectsCount = infoObjectsCount;
            ApduLength = apduLength;
        }

        public static FrameIHeader Read(ReadOnlySpan<byte> buffer, int apduLength)
        {
            var sendSeqNo = (ushort)Bytes.GetNFromBytes(buffer[0], buffer[1]);
            var recieveSeqNo = (ushort)Bytes.GetNFromBytes(buffer[2], buffer[3]);
            var bAsduTypeId = buffer[4];
            var asduTypeId = Enum.IsDefined(typeof(AsduTypeId), bAsduTypeId)
                ? (AsduTypeId)bAsduTypeId
                : AsduTypeId.Unknown;
            var (isSequence, infoObjectsCount) = ReadByte7(buffer[5]);
            var (isTest, isNegative, cot) = ReadByte8(buffer[6]);
            var org = buffer[7];
            var ushortLE = new UshortLittleEndian { LSB = buffer[8], MSB = buffer[9] };
            var ca = ushortLE.V;

            return new FrameIHeader(sendSeqNo, recieveSeqNo, asduTypeId, isSequence, isTest, isNegative, cot, org, ca, infoObjectsCount, apduLength);
        }

        public int TryWrite(Span<byte> buffer)
        {
            // :0 start byte
            buffer[0] = Constants.StartByte;

            // :1 length of APDU
            buffer[1] = (byte)ApduLength;

            // APCI (4 bytes)
            var (b2, b3) = Bytes.GetBytesFromN(SendSeqNo);
            var (b4, b5) = Bytes.GetBytesFromN(RecieveSeqNo);
            buffer[2] = b2;
            buffer[3] = b3;
            buffer[4] = b4;
            buffer[5] = b5;

            // ASDU
            // :6 type identification
            buffer[6] = (byte)AsduTypeId;

            // :7 SQ | number of information objects
            buffer[7] = CreateByte7(IsSequence, InfoObjectsCount);

            // :8 T | P/N | COT
            buffer[8] = CreateByte8(IsTest, IsNegativeConfirmation, CauseOfTransmission);

            // :9 ORG
            buffer[9] = OriginatorAddress;

            // :10-11 CA (2 bytes)
            var ca = new UshortLittleEndian { V = CommonAddress };
            buffer[10] = ca.LSB;
            buffer[11] = ca.MSB;

            return Constants.FrameIHeaderLength;
        }

        internal static (bool isSequence, byte ioCount) ReadByte7(byte b7)
        {
            var isSequence = (b7 & 0x80) != 0;
            int ioCount = 0;
            for (int i = 0; i <= 6; i++)
            {
                if ((b7 & (1 << i)) != 0)
                {
                    ioCount |= 1 << i;
                }
            }

            return (isSequence, (byte)ioCount);
        }

        internal static byte CreateByte7(bool isSequence, int ioCount)
        {
            int b = 0;
            if (isSequence)
            {
                b |= 0x80;
            }

            byte bioc = (byte)ioCount;
            for (int i = 0; i <= 6; i++)
            {
                if ((bioc & (1 << i)) != 0)
                {
                    b |= 1 << i;
                }
            }

            return (byte)b;
        }

        internal static (bool isTest, bool isNegative, CauseOfTransmission cot) ReadByte8(byte b8)
        {
            var isTest = (b8 & 0x80) != 0;
            var isNegative = (b8 & 0x40) != 0;
            int n = 0;
            for (int i = 0; i <= 5; i++)
            {
                if ((b8 & (1 << i)) != 0)
                {
                    n |= 1 << i;
                }
            }

            var cot = Enum.IsDefined(typeof(CauseOfTransmission), (byte)n)
                ? (CauseOfTransmission)n
                : CauseOfTransmission.UNKNOWN;

            return (isTest, isNegative, cot);
        }

        internal static byte CreateByte8(bool isTest, bool isNegative, CauseOfTransmission cot)
        {
            int b = 0;
            if (isTest)
            {
                b |= 0x80;
            }

            if (isNegative)
            {
                b |= 0x40;
            }

            byte bCot = (byte)cot;
            for (int i = 0; i <= 5; i++)
            {
                if ((bCot & (1 << i)) != 0)
                {
                    b |= 1 << i;
                }
            }

            return (byte)b;
        }

        public static readonly Dictionary<int, string> Annotations = new Dictionary<int, string>
        {
            { 0, "start (0x68)" },
            { 1, "APDU length" },
            { 2, "N(S) LSB | 0" },
            { 3, "N(S) MSB" },
            { 4, "N(R) LSB | 0" },
            { 5, "N(R) MSB" },
            { 6, "type identification" },
            { 7, "SQ (b7) | IO count (b0-6)" },
            { 8, "T (b7) | P/N (b6) | COT (b0-5)" },
            { 9, "ORG" },
            { 10, "CA LSB" },
            { 11, "CA MSB" },
        };
    }
}
