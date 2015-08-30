using System;
using System.Configuration;
using System.Threading.Tasks;
using ElasticSearchSample.Models;
using Nest;

namespace ElasticSearchSample.Services
{
    public class LocationSearchService
    {
        private readonly string _connectionUri = ConfigurationManager.AppSettings["ElasticSearchUri"];
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

        public async Task<IIndexResponse> IndexAsync(Location location)
        {
            var client = GetClient();
            return await client.IndexAsync(location);
        }

        public async Task<IIndicesResponse> MapAsync()
        {
            var client = GetClient();
            await client.CreateIndexAsync(_indexName);

            return await client.MapAsync<Location>(m => m.MapFromAttributes());
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

        public async Task<IUpdateResponse> Update(string id, Location location)
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
