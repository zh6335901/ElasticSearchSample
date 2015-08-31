using ElasticSearchSample.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Services
{
    public class LocationSearchService
    {
        private readonly string _connectionUri = "http://localhost:9200/";
        private readonly string _indexName;

        public LocationSearchService(string indexName)
        {
            _indexName = indexName;
        }

        private ElasticClient GetClient()
        {
            var uri = new Uri(_connectionUri);
            var settings = new ConnectionSettings(uri, defaultIndex: _indexName);
            return new ElasticClient(settings);
        }

        public async Task<IEnumerable<LocationSearchResult>> SearchAndSortByGeoAsync(string nameKeyword, GeoPoint from, int startIndex = 0, int size = 10)
        {
            var client = GetClient();

            var response = await client.SearchAsync<Location>(s => s
                .From(startIndex)
                .Size(size)
                .Query(q => q
                    .Match(m => m.OnField(l => l.Name).Query(nameKeyword)))
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
                Longitude = h.Source.GeoPoint.Lon
            });
        }

        public async Task<IIndexResponse> IndexAsync(Location location)
        {
            var client = GetClient();

            if (!(await client.IndexExistsAsync(_indexName)).Exists)
            {
                await MapAsync();
            }

            return await client.IndexAsync(location);
        }

        public async Task<IIndicesResponse> MapAsync()
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

        public async Task<IIndicesResponse> DeleteIndexAsync()
        {
            var client = GetClient();
            var indexNameMaker = new IndexNameMarker()
            {
                Type = typeof(Location),
                Name = _indexName
            };

            return await client.DeleteIndexAsync(new DeleteIndexRequest(indexNameMaker));
        }

        public async Task<IDeleteResponse> DeleteAsync(string id)
        {
            var client = GetClient();
            return await client.DeleteAsync<Location>(id);
        }

        public async Task<IUpdateResponse> UpdateAsync(string id, Location location)
        {
            var client = GetClient();
            var response = await client
                .UpdateAsync<Location, Location>(
                    it => it.Id(id).Doc(location));

            await client.ClearCacheAsync();
            return response;
        }
    }
}
