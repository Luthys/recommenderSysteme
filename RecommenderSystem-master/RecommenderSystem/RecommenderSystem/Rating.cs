using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecommenderSystem
{
    public class Rating
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Rate { get; set; }
    }
}
