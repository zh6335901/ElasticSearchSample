using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearchSample.Models
{
    public class ProductSearchPrompt
    {
        [ElasticProperty(Store = true, Analyzer = "ik", SearchAnalyzer = "ik")]
        public string Name { get; set; }

        [ElasticProperty(AddSortField = true)]
        public int ProductCount { get; set; }
    }
}
