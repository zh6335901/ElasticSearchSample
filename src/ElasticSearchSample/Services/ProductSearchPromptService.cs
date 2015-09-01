using ElasticSearchSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearchSample.Services
{
    public class ProductSearchPromptService : SearchService<ProductSearchPrompt>
    {
        public ProductSearchPromptService(string _indexName, string connectionUri = null) : base(_indexName, connectionUri)
        { }

        public override async Task<IIndicesResponse> MapAsync()
        {
            var client = GetClient();
            await client.CreateIndexAsync(_indexName);

            return await client.MapAsync<ProductSearchPrompt>(m => m
                .MapFromAttributes()
                .Properties(p => p
                    .String(n => n
                        .Name(prompt => prompt.Name)
                        .Store(true)
                        .Analyzer("ik")
                        .SearchAnalyzer("ik")))
                .Properties(p => p
                    .Number(pc => pc
                        .Name(prompt => prompt.ProductCount)
                        .Type(NumberType.Integer))));
        }

        public async Task<IEnumerable<ProductSearchPrompt>> GetProductSearchPrompts(string keyword, int size = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword) || size <= 0)
            {
                return Enumerable.Empty<ProductSearchPrompt>();
            }

            var client = GetClient();

            var response = await client.SearchAsync<ProductSearchPrompt>(sd => sd
                .From(0)
                .Size(size)
                .Query(q => q
                    .Match(md => md
                        .OnField(prompt => prompt.Name)
                        .Operator(Operator.Or)
                        .Query(keyword)))
                .Highlight(h => h
                    .OnFields(fs => fs
                        .OnField(prompt => prompt.Name)
                        .PreTags("<strong>")
                        .PostTags("</strong>")))
                .Filter(f => f.Range(r => r.Greater(0)))
                .Sort(sfd => sfd.UnmappedType(FieldType.Integer).Descending()));

            return response.Documents;
        }
    }
}
