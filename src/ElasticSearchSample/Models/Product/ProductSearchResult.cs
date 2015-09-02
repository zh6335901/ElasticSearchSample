using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Models
{
    public class ProductSearchResult
    {
        public string Name { get; set; }

        public string Picture { get; set; }

        public double MemberPrice { get; set; }

        public double MarketPrice { get; set; }

        public string ShopName { get; set; }

        public double Weight { get; set; }

        public string Id { get; set; }

        public double Score { get; set; }

        public DateTime PublishedTime { get; set; }
    }
}
