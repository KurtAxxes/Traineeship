using AxxesTimes.Data.Models;
using System.Collections.Generic;

namespace AxxesTimes.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
