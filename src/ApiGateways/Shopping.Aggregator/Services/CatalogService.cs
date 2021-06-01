using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client;

        public CatalogService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<CatalogModels> GetCatalog(string id)
        {
            var response = await _client.GetAsync($"/api/v1/Catalog/{id}");

            return await response.ReadContentAs<CatalogModels>();
        }

        public async Task<IEnumerable<CatalogModels>> GetCatalogByCategory(string category)
        {
            var response = await _client.GetAsync($"/api/v1/Catalog/GetProductByCategory/{category}");
            return await response.ReadContentAs<List<CatalogModels>>();
        }

        public async Task<IEnumerable<CatalogModels>> GetCatalog()
        {
            var response = await _client.GetAsync("/api/v1/Catalog");

            return await response.ReadContentAs<List<CatalogModels>>();
        }
    }
}
