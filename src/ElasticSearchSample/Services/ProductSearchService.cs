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

        public async Task<IEnumerable<ProductSearchResult>> SearchProductsWithWeightAsync(
            string keyword, 
            Dictionary<string, object> terms, 
            string sortField = "", 
            bool sortDescending = true, 
            int from = 0, 
            int size = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<ProductSearchResult>();
            }

            var client = GetClient();
            QueryContainer queryDescriptor = GenerateSearchProductQuery(keyword, terms);

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
                Name = GetHightlightFieldValue(h, "Name") ?? h.Source.Name,
                Picture = h.Source.Picture,
                Score = h.Score,
                ShopName = GetHightlightFieldValue(h, "ShopName") ?? h.Source.ShopName,
                Weight = h.Source.Weight,
                PublishedTime = h.Source.PublishedTime
            });
        }

        public async Task<string> GetSearchSuggestionAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return string.Empty;
            }

            var client = GetClient();
            var response = await client.SuggestAsync<Product>(sd => sd.Term("name_suggestion", 
                tsd => tsd
                    .Analyzer("ik")
                    .Text(keyword)
                    .MinWordLength(2)
                    .Size(1)
                    .OnField(p => p.Name)));

            if (response.Suggestions.ContainsKey("name_suggestion"))
            {
                Suggest[] suggests = response.Suggestions["name_suggestion"];
                string suggestionStr = keyword;
                bool @fixed = false;
                int currentPos = -1;

                foreach (var suggest in suggests)
                {
                    var option = suggest.Options.FirstOrDefault();
                    if (option != null && suggest.Offset >= currentPos)
                    {
                        var needReplaced = keyword.Substring(suggest.Offset, suggest.Length);
                        if (needReplaced.ToLower() != option.Text.ToLower())
                        {
                            suggestionStr = suggestionStr.Replace(needReplaced, option.Text);

                            @fixed = true;
                            currentPos = suggest.Offset + suggest.Length;
                        }
                    }
                }
                if (@fixed)
                {
                    return suggestionStr;
                }
            }

            return string.Empty;
        }

        private string GetHightlightFieldValue(IHit<Product> hit, string field)
        {
            field = field.Substring(0, 1).ToLower() + field.Substring(1);
            if (hit.Highlights.ContainsKey(field))
            {
                return hit.Highlights[field].Highlights.FirstOrDefault();
            }

            return null;
        }

        private QueryContainer GenerateSearchProductQuery(string keyword, Dictionary<string, object> terms)
        {
            QueryContainer nameQuery = new QueryStringQuery()
            {
                Query = $"name:{keyword}",
                Analyzer = "ik_syno",
                DefaultField = "name",
            };

            QueryContainer shopNameQuery = new QueryStringQuery()
            {
                Query = $"shopName:{keyword}",
                Analyzer = "ik_syno",
                DefaultField = "shopName",
            };

            QueryContainer queryContainer = nameQuery || shopNameQuery;

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
            return queryDescriptor;
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
