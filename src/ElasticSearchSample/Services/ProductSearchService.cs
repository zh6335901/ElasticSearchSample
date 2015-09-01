using ElasticSearchSample.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Services
{
    public class ProductSearchService : SearchService<Product>
    {
        public ProductSearchService(string indexName, string connectionUri = null) : base(indexName, connectionUri)
        { }
    }
}
