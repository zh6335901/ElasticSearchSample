using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Services
{
    public abstract class SearchService<T> where T : class
    {
        protected readonly string _connectionUri;
        protected readonly string _indexName;

        public SearchService(string indexName, string connectionUri = null)
        {
            _indexName = indexName;
            _connectionUri = connectionUri ?? "http://localhost:9200/";
        }

        protected virtual ElasticClient GetClient()
        {
            var uri = new Uri(_connectionUri);
            var settings = new ConnectionSettings(uri, defaultIndex: _indexName);
            return new ElasticClient(settings);
        }

        public virtual async Task<IIndexResponse> IndexAsync(T doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            var client = GetClient();

            if (!(await client.TypeExistsAsync(_indexName, typeof(T).Name.ToLower())).Exists)
            {
                var response = await MapAsync();
            }

            return await client.IndexAsync(doc);
        }

        public virtual async Task<IIndicesResponse> MapAsync()
        {
            var client = GetClient();
            await client.CreateIndexAsync(_indexName);

            return await client.MapAsync<T>(m => m.MapFromAttributes());
        }

        public virtual async Task<IIndicesResponse> DeleteIndexAsync()
        {
            var client = GetClient();
            var indexNameMaker = new IndexNameMarker()
            {
                Type = typeof(T),
                Name = _indexName
            };

            return await client.DeleteIndexAsync(new DeleteIndexRequest(indexNameMaker));
        }

        public virtual async Task<IDeleteResponse> DeleteAsync(string id)
        {
            var client = GetClient();
            return await client.DeleteAsync<T>(id);
        }

        public virtual async Task<IUpdateResponse> UpdateAsync(string id, T doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            var client = GetClient();
            var response = await client
                .UpdateAsync<T, T>(
                    it => it.Id(id).Doc(doc));

            await client.ClearCacheAsync();
            return response;
        }
    }
}
