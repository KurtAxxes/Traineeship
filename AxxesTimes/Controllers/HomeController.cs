using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AxxesTimes.Models;
using AxxesTimes.Data;
using System.Threading.Tasks;
using System.Linq;
using NServiceBus;
using AxxesTimes.Events;

namespace AxxesTimes.Controllers
{
    public class HomeController : Controller
    {
        private const int PageSize = 5;

        private readonly ILogger<HomeController> _logger;
        private readonly IArticlesRepository _articlesRepository;
        
        public HomeController(ILogger<HomeController> logger, IArticlesRepository articlesRepository)
        {
            _logger = logger;
            _articlesRepository = articlesRepository;
        }

        public IActionResult Index()
        {
            int page = int.TryParse(Request.Query["p"], out page) ? page : 1;

            var articles = _articlesRepository.GetArticles(PageSize + 1, (page - 1) * PageSize);

            var vm = new HomeViewModel()
            {
                Articles = articles.Take(PageSize),
                CurrentPage = page,
                HasPreviousPage = page > 1,
                HasNextPage = articles.Count() > PageSize
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var article = _articlesRepository.GetArticleById(id);

            if (article == null)
            {
                return NotFound(id);
            }

            // already include the current read in viewmodel
            article.Reads++;

            // update current reads for article in the database
            await NotifyArticleReadAsync(article.Id);

            return View(article);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task NotifyArticleReadAsync(int articleId)
        {
            var endpointConfiguration = new EndpointConfiguration("AxxesTimesSite"); // the initiator of the command
            endpointConfiguration.UseTransport<LearningTransport>();
            
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                                                 .ConfigureAwait(false);

            var articleReadEvent = new ArticleRead
            {
                ArticleId = articleId
            };

            await endpointInstance.Publish(articleReadEvent)
                                  .ConfigureAwait(false);
        }
    }
}
