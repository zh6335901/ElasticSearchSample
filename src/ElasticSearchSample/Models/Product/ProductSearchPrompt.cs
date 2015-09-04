using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearchSample.Models
{
    public class ProductSearchPrompt
    {
        [Required]
        [ElasticProperty(Store = true, Analyzer = "ik", SearchAnalyzer = "ik")]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        [ElasticProperty(AddSortField = true)]
        public int ProductCount { get; set; }
    }
}
