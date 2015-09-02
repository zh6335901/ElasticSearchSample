using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearchSample.Models
{
    public class Product
    {
        [ElasticProperty(Store = true, Analyzer = "ik", SearchAnalyzer = "ik")]
        public string Name { get; set; }

        [ElasticProperty(Store = false, Index = FieldIndexOption.NotAnalyzed)]
        public string Picture { get; set; }

        [ElasticProperty(Type = FieldType.Double, AddSortField = true)]
        public double MemberPrice { get; set; }

        [ElasticProperty(Type = FieldType.Double, AddSortField = true)]
        public double MarketPrice { get; set; }

        [ElasticProperty(Store = true, Analyzer = "ik", SearchAnalyzer = "ik")]
        public string ShopName { get; set; }

        [ElasticProperty(Type = FieldType.Date, AddSortField = true)]
        public DateTime PublishedTime { get; set; }

        [ElasticProperty(Type = FieldType.Double)]
        public double Weight { get; set; }
    }
}
