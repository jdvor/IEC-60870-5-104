namespace Iec608705104.Tests
{
    using Iec608705104.Messages;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    public sealed class Payloads : Lazy<Dictionary<string, PayloadCase>>
    {
        public Payloads(string dirPath)
            : base(() => Load(dirPath))
        {
        }

        private static Dictionary<string, PayloadCase> Load(string dirPath)
        {
            var result = new Dictionary<string, PayloadCase>();

            if (string.IsNullOrEmpty(dirPath))
            {
                return result;
            }

            var dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                return result;
            }

            var fileNameRgx = new Regex(@"(?<FrameType>I|S|U)-(?<Name>\w+)\.txt", RegexOptions.Compiled);
            var whitespaceRgx = new Regex(@"\s", RegexOptions.Compiled);
            foreach (var fi in dir.EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly))
            {
                if (fileNameRgx.IsMatch(fi.Name))
                {
                    var contents = File.ReadAllText(fi.FullName);
                    var firstLine = contents.Split('\n')[0];
                    firstLine = whitespaceRgx.Replace(firstLine, string.Empty);
                    var span = new ReadOnlySpan<char>(firstLine.ToCharArray());
                    var bytes = new byte[span.Length / 2];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        var byteSpan = span.Slice(i * 2, 2);
                        bytes[i] = byte.Parse(byteSpan, NumberStyles.HexNumber);
                    }

                    var name = Path.GetFileNameWithoutExtension(fi.Name);
                    var frameType = Enum.Parse<FrameType>(name[0].ToString());
                    var payloadCase = new PayloadCase { Name = name, Bytes = bytes, FrameType = frameType };
                    result.Add(name, payloadCase);
                }
            }

            return result;
        }
    }
}
