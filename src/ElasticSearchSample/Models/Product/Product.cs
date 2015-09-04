using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearchSample.Models
{
    public class Product
    {
        [Required]
        [ElasticProperty(Store = true, Analyzer = "ik", SearchAnalyzer = "ik")]
        public string Name { get; set; }

        [ElasticProperty(Store = false, Index = FieldIndexOption.NotAnalyzed)]
        public string Picture { get; set; }

        [ElasticProperty(Type = FieldType.Double, AddSortField = true)]
        public double MemberPrice { get; set; }

        [ElasticProperty(Type = FieldType.Double, AddSortField = true)]
        public double MarketPrice { get; set; }

        [Required]
        [ElasticProperty(Store = true, Analyzer = "ik", SearchAnalyzer = "ik")]
        public string ShopName { get; set; }

        [ElasticProperty(Type = FieldType.Date, AddSortField = true)]
        public DateTime PublishedTime { get; set; }

        [ElasticProperty(Type = FieldType.Double)]
        public double Weight { get; set; }
    }
}
