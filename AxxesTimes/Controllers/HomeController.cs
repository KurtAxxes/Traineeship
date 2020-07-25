using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AxxesTimes.Models;
using AxxesTimes.Data;
using System.Threading.Tasks;
using System.Linq;

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
            UpdateArticleRead(article.Id);
            
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


        /*
         * THIS APPROACH IS A BAD IDEA BECAUSE IT'S A DIRECT DEPENDENCY ON THE DATABASE
         * AND CAN POTENTIALLY HAVE A HUGE PERFORMANCE IMPACT.
         * IT IS BETTER TO USE MESSAGE QUEUES INSTEAD.
         */
        private void UpdateArticleRead(int articleId)
        {
            _articlesRepository.UpdateArticleRead(articleId);
        }
    }
}
