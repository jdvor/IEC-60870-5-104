namespace Iec608705104
{
    using System.Diagnostics.Tracing;

    [EventSource(Name = nameof(Iec608705104))]
    internal sealed class EvtSrc : EventSource
    {
        internal static readonly EvtSrc Log = new EvtSrc();

        private EvtSrc()
            : base()
        { }

        [Event(
            EventIds.ServiceBusMsgPublished,
            Keywords = Keywords.Publishing,
            Level = EventLevel.Informational,
            Message = "ServiceBus message published (id: {0}, success: {1}, time taken: {2} ms, message size: {3} bytes)")]
        internal void MsgPublished(string messageId, bool success, long elapsedMs, int messageSize)
        {
            if (IsEnabled())
            {
                WriteEvent(EventIds.ServiceBusMsgPublished, messageId, success, elapsedMs, messageSize);
            }
        }

        internal static class Keywords
        {
            public const EventKeywords Serialization = (EventKeywords)(1 << 1);
            public const EventKeywords Compression = (EventKeywords)(1 << 2);
            public const EventKeywords Publishing = (EventKeywords)(1 << 3);
        }

        internal static class EventIds
        {
            public const int ServiceBusMsgPublished = 101;
        }
    }
}
