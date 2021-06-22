using IdentityServer.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Store
{
    public class CustomResourceStore : IResourceStore
    {
        protected IMongoRepository _dbRepository;

        public CustomResourceStore(IMongoRepository repository)
        {
            _dbRepository = repository;
        }

        private IEnumerable<ApiResource> GetAllApiResources()
        {
            return _dbRepository.All<ApiResource>();
        }

        private IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return _dbRepository.All<IdentityResource>();
        }

        private IEnumerable<ApiScope> GetAllApiScopes()
        {
            return _dbRepository.All<ApiScope>();
        }

        private Func<IdentityResource, bool> BuildPredicate(Func<IdentityResource, bool> predicate)
        {
            return predicate;
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(GetAllIdentityResources(), GetAllApiResources(),GetAllApiScopes());
            return Task.FromResult(result);
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            //var list = await _dbRepository.Where<IdentityResource>(e => scopeNames.Contains(e.Name)).ToListAsync();
            return Task.FromResult(_dbRepository.Where<IdentityResource>(e => scopeNames.Contains(e.Name)).AsEnumerable());
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            //var list = await _dbRepository.Where<ApiScope>(e => scopeNames.Contains(e.Name)).ToListAsync();
            return Task.FromResult(_dbRepository.Where<ApiScope>(e => scopeNames.Contains(e.Name)).AsEnumerable());
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            //var list = await _dbRepository.Where<ApiResource>(a => a.Scopes.Any(s => scopeNames.Contains(s))).ToListAsync();
            return Task.FromResult(_dbRepository.Where<ApiResource>(a => a.Scopes.Any(s => scopeNames.Contains(s))).AsEnumerable());
        }

        public  Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            //var list = await _dbRepository.Where<ApiResource>(a => a.Scopes.Any(s => apiResourceNames.Contains(s))).ToListAsync();
            return Task.FromResult(_dbRepository.Where<ApiResource>(a => a.Scopes.Any(s => apiResourceNames.Contains(s))).AsEnumerable());
        }
    }
}
