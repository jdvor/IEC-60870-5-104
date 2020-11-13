namespace Iec608705104
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ByteUtil
    {
        public static string Describe(ReadOnlySpan<byte> bytes, int offset = 0, int length = 0, bool msbToLsb = true, Dictionary<int, string> annotations = null)
        {
            if (bytes == null
                || bytes.Length == 0
                || offset < 0
                || bytes.Length < offset)
            {
                return string.Empty;
            }

            var actualLength = length <= 0
                ? bytes.Length - offset
                : length;
            var maxAnnotationLength = annotations != null && annotations.Count > 0
                ? annotations.Values.Max(x => x.Length)
                : 0;
            int maxLineLength;
            string footer;
            bool hasAnnotations;
            if (maxAnnotationLength > 0)
            {
                maxLineLength = 25 + (maxAnnotationLength > 4 ? maxAnnotationLength + 2 : 6);
                var expandFooterBy = maxLineLength - 25;
                footer = Footer(expandFooterBy);
                hasAnnotations = true;
            }
            else
            {
                maxLineLength = 25;
                footer = Footer();
                hasAnnotations = false;
            }

            var sb = new StringBuilder((actualLength + 4) * maxLineLength);
            sb.Append(footer);
            sb.AppendLine();
            sb.Append("  n    hex  dec  ");
            sb.Append(msbToLsb ? "76543210" : "01234567");
            if (hasAnnotations)
            {
                sb.AppendLine("  note");
            }
            else
            {
                sb.AppendLine();
            }

            sb.Append(footer);
            sb.AppendLine();

            int n;
            int i;
            for (n = offset, i = 0; n < offset + actualLength; n++, i++)
            {
                var b = bytes[n];
                sb.AppendFormat("{0,3}   0x{1:X2}  {2,3}  {3}", n, b, b, ToBitString(b, msbToLsb));
                if (hasAnnotations && annotations.TryGetValue(i, out var note))
                {
                    sb.Append("  ");
                    sb.Append(note);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string Footer(int expandFooterBy = 0)
        {
            const string footer = @"-------------------------";
            if (expandFooterBy == 0)
            {
                return footer;
            }

            var sb = new StringBuilder(25 + expandFooterBy);
            sb.Append(footer);
            for (int i = 0; i < expandFooterBy; i++)
            {
                sb.Append('-');
            }

            return sb.ToString();
        }

        public static string ToBitString(byte b, bool msbToLsb = true)
        {
            var chars = new char[8];
            for (int i = 0; i <= 7; i++)
            {
                var idx = msbToLsb ? 7 - i : i;
                chars[idx] = (b & (1 << i)) != 0 ? '1' : '0';
            }

            return new string(chars);
        }
    }
}
