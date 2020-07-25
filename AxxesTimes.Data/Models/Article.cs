using System;
using System.ComponentModel.DataAnnotations;

namespace AxxesTimes.Data.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Intro { get; set; }
        public string Image { get; set; }
        public string BodyHtml { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int Reads { get; set; }
    }
}
