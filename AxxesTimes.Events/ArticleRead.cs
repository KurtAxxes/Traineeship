using NServiceBus;

namespace AxxesTimes.Events
{
    public class ArticleRead : IEvent
    {
        public int ArticleId { get; set; }
    }
}
