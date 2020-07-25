using AxxesTimes.Data;
using AxxesTimes.Events;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;

namespace ArticleReadSubscriber.Handlers
{
    class ArticleReadHandler : IHandleMessages<ArticleRead>
    {
        static readonly ILog log = LogManager.GetLogger<ArticleReadHandler>();
        static Random random = new Random();

        private readonly IArticlesRepository _articleRepository;

        public ArticleReadHandler(IArticlesRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Task Handle(ArticleRead message, IMessageHandlerContext context)
        {
            var articleId = message.ArticleId;

            log.Info($"Received message for article {articleId}.");
            
            // generate system error, always happening
            //throw new Exception("BOOM");

            _articleRepository.UpdateArticleRead(articleId);

            log.Info($"Processed article read for article id {articleId}");

            return Task.CompletedTask;
        }
    }
}
