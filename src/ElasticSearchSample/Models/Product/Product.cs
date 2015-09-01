using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Models.Product
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Picture { get; set; }

        public string MemberPrice { get; set; }

        public string MarketPrice { get; set; }

        public string ShopName { get; set; }

        public double Boost { get; set; }
    }

    public enum SortField
    {
        None = 0,
        Price,
        Credit,
        Volume,
        Published
    }
}
