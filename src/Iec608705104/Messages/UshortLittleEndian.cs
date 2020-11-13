namespace Iec608705104.Messages
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    internal ref struct UshortLittleEndian
    {
        [FieldOffset(0)]
        public ushort V;

        [FieldOffset(0)]
        public byte LSB;

        [FieldOffset(1)]
        public byte MSB;
    }
}
