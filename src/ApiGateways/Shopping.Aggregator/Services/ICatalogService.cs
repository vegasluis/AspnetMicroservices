using System.Collections.Generic;
using System.Threading.Tasks;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogModels>> GetCatalog();
        Task<IEnumerable<CatalogModels>> GetCatalogByCategory(string category);
        Task<CatalogModels> GetCatalog(string id);
    }
}
