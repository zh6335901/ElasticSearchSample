using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Models
{
    public class LocationSearchResult
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }

        public float Distance { get; set; }
    }
}

