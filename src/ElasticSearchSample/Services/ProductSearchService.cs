using ElasticSearchSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using System.Linq.Expressions;

namespace ElasticSearchSample.Services
{
    public class ProductSearchService : SearchService<Product>
    {
        public ProductSearchService(string indexName, string connectionUri = null) : base(indexName, connectionUri)
        { }

        public async Task<IEnumerable<ProductSearchResult>> SearchProductsWithWeight(
            string keyword, 
            Dictionary<string, object> terms, 
            string sortField = "", 
            bool sortDescending = true, 
            int from = 0, 
            int size = 10)
        {
            var client = GetClient();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<ProductSearchResult>();
            }

            QueryContainer queryContainer = new MatchQuery()
            {
                Analyzer = "ik",
                Operator = Operator.And,
                Field = "name",
                Query = keyword
            };

            if (terms != null)
            {
                foreach (var term in terms)
                {
                    QueryContainer termQuery = new TermQuery
                    { 
                        Field = term.Key,
                        Value = term.Value
                    };

                    queryContainer = queryContainer && termQuery;
                }
            }

            var queryDescriptor = new QueryDescriptor<Product>()
                .FunctionScore(fsq => fsq
                    .Query(q => queryContainer)
                    .BoostMode(FunctionBoostMode.Replace)
                    .ScriptScore(script => script.Script("_score + doc['weight']")));

            var searchDescriptor = new SearchDescriptor<Product>()
                .From(from)
                .Size(size)
                .Query(queryDescriptor)
                .TrackScores(true)
                .Highlight(h => h
                    .OnFields(
                        fields => ConfigHighlightFieldDescriptor(fields, p => p.Name),
                        fields => ConfigHighlightFieldDescriptor(fields, p => p.ShopName)));

            if (!string.IsNullOrEmpty(sortField))
            {
                searchDescriptor = searchDescriptor
                    .Sort(sort => sort
                    .OnField(sortField)
                    .Order(sortDescending ? SortOrder.Descending : SortOrder.Ascending));
            }

            var response = await client.SearchAsync<Product>(searchDescriptor);

            return response.Hits.Select(h => new ProductSearchResult
            {
                Id = h.Id,
                MarketPrice = h.Source.MarketPrice,
                MemberPrice = h.Source.MemberPrice,
                Name = h.Highlights["name"].Highlights.FirstOrDefault() ?? h.Source.Name,
                Picture = h.Source.Picture,
                Score = h.Score,
                ShopName = h.Source.ShopName,
                Weight = h.Source.Weight,
                PublishedTime = h.Source.PublishedTime
            });
        }

        private void ConfigHighlightFieldDescriptor(HighlightFieldDescriptor<Product> descriptor, Expression<Func<Product, object>> expression)
        {
            descriptor
                .OnField(expression)
                .PreTags("<strong>")
                .PostTags("</strong>");
        }
    }
}
