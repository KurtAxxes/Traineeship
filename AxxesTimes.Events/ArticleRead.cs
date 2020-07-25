namespace AxxesTimes.Events
{
    // Implement message as NServiceBus event
    public class ArticleRead
    {
        public int ArticleId { get; set; }
    }
}
