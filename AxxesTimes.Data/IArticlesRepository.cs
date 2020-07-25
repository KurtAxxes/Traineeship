using AxxesTimes.Data.Models;
using System.Collections.Generic;

namespace AxxesTimes.Data
{
    public interface IArticlesRepository
    {
        IEnumerable<Article> GetArticles(int amount, int skipAmount = 0);

        Article GetArticleById(int articleId);

        void UpdateArticleRead(int articleId);
    }
}
