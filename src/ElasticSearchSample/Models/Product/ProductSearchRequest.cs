using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Models
{
    public class ProductSearchRequest
    {
        public string Keyword { get; set; }

        public Dictionary<string, object> Terms { get; set; }
        
        public string SortField { get; set; }

        public bool SortDescending { get; set; } = true;

        public int From { get; set; } = 0;

        public int Size { get; set; } = 10;
    }
}
