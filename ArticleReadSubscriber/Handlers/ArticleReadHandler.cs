using AxxesTimes.Data;
using AxxesTimes.Events;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace ArticleReadSubscriber.Handlers
{
    class ArticleReadHandler : IHandleMessages<ArticleRead>
    {
        static readonly ILog log = LogManager.GetLogger<ArticleReadHandler>();

        private readonly IArticlesRepository _articleRepository;

        public ArticleReadHandler(IArticlesRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Task Handle(ArticleRead message, IMessageHandlerContext context)
        {
            var articleId = message.ArticleId;

            _articleRepository.UpdateArticleRead(articleId);

            log.Info($"Processed article read for article id {articleId}");

            return Task.CompletedTask;
        }
    }
}
