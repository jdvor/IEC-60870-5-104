namespace Iec608705104
{
    using Iec608705104.Messages;
    using Iec608705104.Messages.Asdu;
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;

    internal static class Extensions
    {
        [Conditional("DEBUG")]
        internal static void Debug(this SequenceReader<byte> r, string msg, int apduLength = 0)
        {
            var al = apduLength > 0 ? $", apdu: {apduLength}" : string.Empty;
            System.Diagnostics.Debug.WriteLine(
                $"{msg} pos: {r.Position.GetInteger()}{al}, rem: {r.Remaining}, len: {r.Length}, ss: {(r.Sequence.IsSingleSegment ? 1 : 0)}");
        }

        [Conditional("DEBUG")]
        internal static void Debug(this ReadOnlySequence<byte> bytes, string msg, bool byte2hex = false)
            => Debug(bytes.ToArray(), msg, byte2hex);

        [Conditional("DEBUG")]
        internal static void Debug(this Span<byte> bytes, string msg, bool byte2hex = false)
            => Debug((ReadOnlySpan<byte>)bytes, msg, byte2hex);

        [Conditional("DEBUG")]
        internal static void Debug(this ReadOnlySpan<byte> bytes, string msg, bool byte2hex = false)
            => System.Diagnostics.Debug.WriteLine(ToDebugString(bytes, msg, byte2hex));

        internal static string ToDebugString(this Span<byte> bytes, string msg = null, bool byte2hex = false)
            => ToDebugString((ReadOnlySpan<byte>)bytes, msg, byte2hex);

        internal static string ToDebugString(this ReadOnlySpan<byte> bytes, string msg = null, bool byte2hex = false)
        {
            var msgIsEmpty = string.IsNullOrEmpty(msg);
            var sb = new StringBuilder((msgIsEmpty ? 0 : msg.Length) + 5 + (bytes.Length * 5));
            if (!msgIsEmpty)
            {
                sb.Append(msg);
                sb.Append(' ');
            }

            sb.Append("[ ");
            var first = true;
            foreach (var b in bytes)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(' ');
                }

                if (byte2hex)
                {
                    sb.Append(b.ToString("X2"));
                }
                else
                {
                    sb.Append(b);
                }
            }

            sb.Append(" ]");

            return sb.ToString();
        }

        internal static void ShutdownAndClose(this Socket socket, int closeTimeoutSecs)
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    socket.Close(closeTimeoutSecs);
                }
            }
        }

        internal static int CalculateApduLength<T>(this InfoObject<T>[] infoObjects, int addressLength = Constants.InfoObjectAddressLength)
            where T : IAsduValue, new()
        {
            return Constants.NoPrefix.FrameIHeaderLength + infoObjects.Sum(x => x.Value.EncodedLength + addressLength);
        }
    }
}
