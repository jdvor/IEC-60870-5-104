namespace Iec608705104.Messages
{
    using Iec608705104.Messages.Asdu;

    internal static class Cache<T>
        where T : IAsduValue, new()
    {
        internal static readonly InfoObjectMeta Meta = new T().CreateMeta();
    }
}
