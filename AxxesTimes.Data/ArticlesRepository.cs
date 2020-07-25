using AxxesTimes.Data.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AxxesTimes.Data
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly string _connectionString;

        public ArticlesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public IEnumerable<Article> GetArticles(int amount, int skipAmount = 0)
        {
            const string query = @"SELECT a.Id, a.Title, a.Image, a.Intro, a.BodyHtml, a.Date, IsNull(ar.Count, 0) AS Reads
                                   FROM dbo.Article AS a WITH (NOLOCK)
                                   LEFT OUTER JOIN dbo.ArticleReads AS ar WITH (NOLOCK) ON ar.ArticleId = a.Id
                                   ORDER BY a.Date DESC
                                   OFFSET @SkipAmount ROWS FETCH NEXT @Amount ROWS ONLY;";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var queryParameters = new
                {
                    SkipAmount = skipAmount,
                    Amount = amount
                };

                return conn.Query<Article>(query, queryParameters);
            }
        }

        public Article GetArticleById(int articleId)
        {
            const string query = @"SELECT a.Id, a.Title, a.Image, a.Intro, a.BodyHtml, a.Date, IsNull(ar.Count, 0) AS Reads
                                   FROM dbo.Article AS a WITH (NOLOCK)
                                   LEFT OUTER JOIN dbo.ArticleReads AS ar WITH (NOLOCK) ON ar.ArticleId = a.Id
                                   WHERE a.Id = @ArticleId;";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var queryParameters = new
                {
                    ArticleId = articleId
                };

                return conn.QueryFirstOrDefault<Article>(query, queryParameters);
            }
        }

        public void UpdateArticleRead(int articleId)
        {
            const string query = @"IF EXISTS ( SELECT * FROM dbo.ArticleReads WITH (UPDLOCK) WHERE ArticleId = @ArticleId )
                                       UPDATE dbo.ArticleReads
                                         SET Count = Count + 1
                                       WHERE ArticleId = @ArticleId;
 
                                    ELSE 
 
                                      INSERT dbo.ArticleReads ( ArticleId, Count )
                                      VALUES ( @ArticleId, 1 );";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var queryParameters = new
                {
                    ArticleId = articleId
                };

                conn.Query(query, queryParameters);
            }
        }
    }
}
