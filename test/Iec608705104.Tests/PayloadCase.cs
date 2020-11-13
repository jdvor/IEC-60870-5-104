namespace Iec608705104.Tests
{
    using Iec608705104.Messages;
    using System;
    using Xunit.Abstractions;

    public sealed class PayloadCase : IXunitSerializable
    {
        public string Name { get; set; }

        public byte[] Bytes { get; set; }

        public ReadOnlySpan<byte> Span => Bytes.AsSpan();

        public ReadOnlySpan<byte> SpanWithoutPrefix => Bytes.AsSpan().Slice(2);

        public FrameType FrameType { get; set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Name = info.GetValue<string>("name");
            Bytes = info.GetValue<byte[]>("bytes");
            FrameType = info.GetValue<FrameType>("frameType");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("name", Name);
            info.AddValue("bytes", Bytes);
            info.AddValue("frameType", FrameType);
        }

        public override string ToString() => Name;
    }
}
