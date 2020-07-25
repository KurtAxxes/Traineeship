using AxxesTimes.Data;

namespace ReadArticleSubscriber.Handlers
{
    // Handle ReadArticle messages with NServiceBus here
    class ReadArticleHandler
    {
        static readonly ILog log = LogManager.GetLogger<ReadArticleHandler>();
        private readonly IArticlesRepository _articleRepository;

        public ReadArticleHandler(IArticlesRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        // Handle ReadArticle messages
        // - Update article reads in the database for article message.
        // - Use the logger to output an info message to the console window when an article has been processed.
    }
}
