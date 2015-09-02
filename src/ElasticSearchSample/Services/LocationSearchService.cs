using ElasticSearchSample.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Services
{
    public class LocationSearchService : SearchService<Location>
    {
        public LocationSearchService(string indexName, string connectionUri = null) : base(indexName, connectionUri)
        {
        }

        public async Task<IEnumerable<LocationSearchResult>> SearchAndSortByGeoAsync(
            string nameKeyword, 
            GeoPoint from, 
            int startIndex = 0, 
            int size = 10)
        {
            var client = GetClient();

            var response = await client.SearchAsync<Location>(s => s
                .From(startIndex)
                .Size(size)
                .Query(q => q
                    .Match(m => m
                        .OnField(l => l.Name)
                        .Operator(Operator.And)
                        .Query(nameKeyword)))
                .SortGeoDistance(sort => sort
                    .OnField(l => l.GeoPoint)
                    .PinTo(from.Lat, from.Lon)
                    .Unit(GeoUnit.Kilometers)
                    .Ascending()));

            return response.Hits.Select(h => new LocationSearchResult
            {
                Name = h.Source.Name,
                Distance = Convert.ToSingle(h.Sorts.First()),
                Latitude = h.Source.GeoPoint.Lat,
                Longitude = h.Source.GeoPoint.Lon,
                Id = h.Id
            });
        }

        public override async Task<IIndicesResponse> MapAsync()
        {
            var client = GetClient();
            await client.CreateIndexAsync(_indexName);

            return await client.MapAsync<Location>(m => m
                .MapFromAttributes()
                .Properties(p => p
                    .String(n => n
                        .Name(l => l.Name)
                        .Analyzer("ik")
                        .SearchAnalyzer("ik")))
                .Properties(p => p
                    .GeoPoint(l => l
                        .Name(it => it.GeoPoint)
                        .IndexGeoHash()
                        .IndexLatLon())));
        }
    }
}
