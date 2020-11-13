namespace Iec608705104.Messages
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    internal ref struct Int32LittleEndian
    {
        [FieldOffset(0)]
        public int V;

        [FieldOffset(0)]
        public byte B0;

        [FieldOffset(1)]
        public byte B1;

        [FieldOffset(2)]
        public byte B2;
    }
}
