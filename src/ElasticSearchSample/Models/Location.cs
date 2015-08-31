using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using System.ComponentModel.DataAnnotations;

namespace ElasticSearchSample.Models
{
    public class Location
    {
        public string Id { get; set; }

        [Required]
        [ElasticProperty(Analyzer = "ik", SearchAnalyzer = "ik", AddSortField = true)]
        public string Name { get; set; }

        [ElasticProperty(Type = FieldType.GeoPoint)]
        public GeoPoint GeoPoint { get; set; }
    }

    public class GeoPoint
    {
        public GeoPoint(float lon, float lat)
        {
            Lon = lon;
            Lat = lat;
        }

        public GeoPoint() : this(0, 0) { }

        public float Lon { get; set; }

        public float Lat { get; set; }

        public override string ToString() => $"{Lon}, {Lat}";
    }
}
