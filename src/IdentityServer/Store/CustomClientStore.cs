using IdentityServer.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Threading.Tasks;

namespace IdentityServer.Store
{
    public class CustomClientStore : IClientStore
    {
        protected IMongoRepository _dbRepository;

        public CustomClientStore(IMongoRepository repository)
        {
            _dbRepository = repository;
        }

        public  Task<Client> FindClientByIdAsync(string clientId)
        {
            var client =  _dbRepository.Single<Client>(c => c.ClientId == clientId);
            return Task.FromResult(client);
        }
    }
}
