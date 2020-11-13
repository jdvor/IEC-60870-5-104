namespace Iec608705104.Messages
{
    using Iec608705104.Messages.Asdu;
    using System;

    public sealed class FrameI<T> : IFrame, IExpectReceival, IConfirmRecieval
        where T : IAsduValue, new()
    {
        public FrameIHeader Header { get; }

        public InfoObject<T>[] InfoObjects { get; }

        public int ApduLength
            => Header.ApduLength;

        public FrameType Type { get; } = FrameType.I;

        public ushort SendSeqNo => Header.SendSeqNo;

        public ushort RecieveSeqNo => Header.RecieveSeqNo;

        public FrameI(FrameIHeader header, InfoObject<T>[] infoObjects)
        {
            Expect.NotNull(header, nameof(header));
            Expect.NotNull(infoObjects, nameof(infoObjects));
            Header = header;
            InfoObjects = infoObjects;
        }

        public int TryWrite(Span<byte> buffer)
        {
            var bytesWritten = Header.TryWrite(buffer);
            if (bytesWritten != Constants.FrameIHeaderLength)
            {
                return -1;
            }

            var currOffset = bytesWritten;
            foreach (var infoObject in InfoObjects)
            {
                if (!Header.IsSequence)
                {
                    var intLE = new Int32LittleEndian { V = infoObject.Address };
                    buffer[currOffset] = intLE.B0;
                    buffer[currOffset + 1] = intLE.B1;
                    buffer[currOffset + 2] = intLE.B2;
                    currOffset += 3;
                }

                var span = buffer.Slice(currOffset, infoObject.Value.EncodedLength);
                bytesWritten = infoObject.Value.TryWrite(span);
                if (bytesWritten != infoObject.Value.EncodedLength)
                {
                    return -1;
                }

                currOffset += bytesWritten;
            }

            return currOffset;
        }
    }
}
