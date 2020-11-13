namespace Iec608705104
{
    using Iec608705104.Messages;
    using Iec608705104.Messages.Asdu;
    using System;

    internal static class Bytes
    {
        /// <param name="buffer">starts at the position 2 of the whole frame</param>
        internal static (ErrorCode error, FrameType type, AsduTypeId asduTypeId) IdentifyFrame(ReadOnlySpan<byte> buffer)
        {
            var isSorU = buffer.Length == Constants.NoPrefix.MinFrameLength;
            var isI = buffer.Length >= Constants.NoPrefix.MinFrameILength;
            if (!isSorU && !isI)
            {
                return (ErrorCode.BufferDoesNotFit, FrameType.Unknown, AsduTypeId.Unknown);
            }

            var byte2Bit0 = (buffer[0] & 0x01) != 0;
            var byte4Bit0 = (buffer[2] & 0x01) != 0;
            if (isI && !byte2Bit0 && !byte4Bit0)
            {
                var byte6 = buffer[4];
                if (!Enum.IsDefined(typeof(AsduTypeId), byte6))
                {
                    return (ErrorCode.InvalidAsduType, FrameType.I, AsduTypeId.Unknown);
                }

                return (ErrorCode.None, FrameType.I, (AsduTypeId)byte6);
            }
            else if (isSorU)
            {
                var byte2Bit1 = (buffer[0] & 0x02) != 0;
                if (byte2Bit1 && !byte2Bit0 && !byte4Bit0)
                {
                    return (ErrorCode.None, FrameType.S, AsduTypeId.Unknown);
                }

                if (byte2Bit1 && byte2Bit0 && !byte4Bit0)
                {
                    return (ErrorCode.None, FrameType.U, AsduTypeId.Unknown);
                }
            }

            return (ErrorCode.NonIdentifiableFrame, FrameType.Unknown, AsduTypeId.Unknown);
        }

        internal static (ErrorCode, IFrame) ReadFrame(ReadOnlySpan<byte> buffer)
        {
            var apduLength = buffer.Length;
            var (error, frameType, asduTypeId) = IdentifyFrame(buffer);
            if (error != ErrorCode.None)
            {
                return (error, null);
            }

            return frameType switch
            {
                FrameType.I => asduTypeId switch
                {
                    AsduTypeId.C_CS_NA_1 => ReadFrameI<C_CS_NA_1>(buffer, apduLength),
                    _ => (ErrorCode.NotSupportedAsduType, null),
                },
                FrameType.S => FrameS.Read(buffer),
                FrameType.U => FrameU.Read(buffer),
                _ => (ErrorCode.NotSupportedFrameType, null),
            };
        }

        internal static (ErrorCode, FrameI<T>) ReadFrameI<T>(ReadOnlySpan<byte> buffer, int apduLength)
            where T : IAsduValue, new()
        {
            var meta = Cache<T>.Meta;

            var header = FrameIHeader.Read(buffer, apduLength);
            var infoObjects = new InfoObject<T>[header.InfoObjectsCount];
            var currOffset = 10;
            for (int i = 0; i < header.InfoObjectsCount; i++)
            {
                int address;
                if (header.IsSequence)
                {
                    address = header.CommonAddress + i;
                }
                else
                {
                    address = buffer[currOffset] | (buffer[currOffset + 1] << 8) | (buffer[currOffset + 2] << 16);
                    currOffset += 3;
                }

                var value = new T();
                var span = buffer.Slice(currOffset, meta.EncodedLength);
                var error = value.TryRead(span);
                if (error != ErrorCode.None)
                {
                    return (error, null);
                }

                currOffset += meta.EncodedLength;
                infoObjects[i] = new InfoObject<T>(address, value);
            }

            return (ErrorCode.None, new FrameI<T>(header, infoObjects));
        }

        internal static (byte lsb, byte msb) GetBytesFromN(int n)
        {
            int lsb = 0;
            for (var i = 1; i <= 7; i++)
            {
                if ((n & (1 << (i - 1))) != 0)
                {
                    lsb |= 1 << i;
                }
            }

            int msb = 0;
            for (var i = 0; i <= 7; i++)
            {
                if ((n & (1 << (i + 7))) != 0)
                {
                    msb |= 1 << i;
                }
            }

            return ((byte)lsb, (byte)msb);
        }

        internal static int GetNFromBytes(byte lsb, byte msb)
        {
            int n = 0;
            for (int i = 1; i <= 7; i++)
            {
                if ((lsb & (1 << i)) != 0)
                {
                    n |= 1 << (i - 1);
                }
            }

            for (int i = 0; i <= 7; i++)
            {
                if ((msb & (1 << i)) != 0)
                {
                    n |= 1 << (i + 7);
                }
            }

            return n;
        }
    }
}
