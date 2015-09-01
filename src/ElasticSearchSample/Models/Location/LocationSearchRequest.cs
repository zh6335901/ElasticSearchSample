using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Models
{
    public class LocationSearchRequest
    {
        public string Keyword { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }
    }
}
