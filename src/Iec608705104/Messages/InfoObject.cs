namespace Iec608705104.Messages
{
    using Iec608705104.Messages.Asdu;

    public sealed class InfoObject<T>
        where T : IAsduValue, new()
    {
        public int Address { get; }

        public T Value { get; }

        public InfoObject(int address, T value)
        {
            Address = address;
            Value = value;
        }
    }
}
