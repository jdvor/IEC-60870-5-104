namespace Iec608705104.Tests
{
    using Iec608705104.Messages;
    using Iec608705104.Messages.Asdu;
    using KellermanSoftware.CompareNetObjects;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class SerializationTests
    {
        private static readonly CompareLogic cmp = new CompareLogic();
        private static readonly Payloads payloads = new Payloads("./Payloads");

        [Theory]
        [InlineData("2020-02-20T15:49:17.459", true, false)]
        [InlineData("2020-03-29T02:00:00.000", false, true)]
        [InlineData("2020-06-02T23:18:47.824", true, true)]
        [InlineData("2020-10-25T02:59:59.999", false, true)]
        [InlineData("2020-11-13T02:35:57.224", false, false)]
        public void Time(string t, bool isSubstituted, bool isSummerTime)
        {
            var dt = DateTime.Parse(t);
            var expected = new CP56Time2a(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, isSubstituted, isSummerTime);
            var buffer = new byte[7].AsSpan();
            expected.Write(buffer);
            var actual = CP56Time2a.ReadBuffer(buffer);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FrameU_StartDtAct()
        {
            const ControlFunc cf = ControlFunc.STARTDT_ACT;
            var frm1 = new FrameU(cf);

            var buffer = new byte[Constants.MinFrameLength].AsSpan();
            var bytesWritten = frm1.TryWrite(buffer);
            var desc = ByteUtil.Describe(buffer, 0, length: Constants.MinFrameLength, annotations: FrameU.Annotations);

            var (error, frm2) = FrameU.Read(buffer.Slice(2));

            Assert.Equal(ErrorCode.None, error);
            Assert.Equal(cf, frm2.ControlFunc);
        }

        [Fact]
        public void FrameS_305()
        {
            const ushort recieveSeqNo = 305;
            var frm1 = new FrameS(recieveSeqNo);

            var buffer = new byte[Constants.MinFrameLength].AsSpan();
            var bytesWritten = frm1.TryWrite(buffer);
            var desc = ByteUtil.Describe(buffer, 0, length: Constants.MinFrameLength, annotations: FrameS.Annotations);

            var (error, frm2) = FrameS.Read(buffer.Slice(2));

            Assert.Equal(ErrorCode.None, error);
            Assert.Equal(recieveSeqNo, frm2.RecieveSeqNo);
        }

        [Fact]
        public void FrameIHeader_1()
        {
            const int apduLength = 28;
            var bufferLength = Constants.FrameIHeaderLength;
            var h1 = new FrameIHeader(
                sendSeqNo: 121,
                recieveSeqNo: 572,
                asduTypeId: AsduTypeId.M_SP_TA_1,
                isSequence: false,
                isTest: true,
                isNegativeConfirmation: false,
                causeOfTransmission: CauseOfTransmission.REQUEST,
                originatorAddress: 10,
                commonAddress: 1456,
                infoObjectsCount: 6,
                apduLength: apduLength);

            var buffer = new byte[bufferLength].AsSpan();
            var bytesWritten = h1.TryWrite(buffer);
            var desc = ByteUtil.Describe(buffer, 0, length: bufferLength, annotations: FrameIHeader.Annotations);

            var h2 = FrameIHeader.Read(buffer.Slice(2), apduLength);

            var result = cmp.Compare(h1, h2);
            Assert.True(result.AreEqual, result.DifferencesString);
        }

        [Theory]
        [MemberData(nameof(PayloadCases))]
        public void DeserializePayloadCases(PayloadCase @case)
        {
            var (error, frame) = Bytes.ReadFrame(@case.SpanWithoutPrefix);
            Assert.Equal(ErrorCode.None, error);
            Assert.Equal(@case.FrameType, frame.Type);
        }

        public static IEnumerable<object[]> PayloadCases()
        {
            foreach (var @case in payloads.Value.Values)
            {
                yield return new object[] { @case };
            }
        }

        [Fact]
        public void Experiment1()
        {
            var value = new C_CS_NA_1(new CP56Time2a(2020, 11, 13, 16, 31, 42, 845, false, false));
            var infoObjects = new[] { new InfoObject<C_CS_NA_1>(3, value) };
            var apduLength = infoObjects.CalculateApduLength();
            var bufferLength = Constants.PrefixLength + apduLength;
            var h = new FrameIHeader(
                sendSeqNo: 1,
                recieveSeqNo: 0,
                asduTypeId: AsduTypeId.C_CS_NA_1,
                isSequence: false,
                isTest: true,
                isNegativeConfirmation: false,
                causeOfTransmission: CauseOfTransmission.ACTIVATION,
                originatorAddress: 10,
                commonAddress: 2,
                infoObjectsCount: infoObjects.Length,
                apduLength: apduLength);
            var frame = new FrameI<C_CS_NA_1>(h, infoObjects);

            var buffer = new byte[bufferLength].AsSpan();
            var bytesWritten = frame.TryWrite(buffer);

            var bytesAsHex = buffer.ToDebugString(byte2hex: true);
            var desc = ByteUtil.Describe(buffer, 0, length: bufferLength, annotations: FrameIHeader.Annotations);
        }
    }
}
