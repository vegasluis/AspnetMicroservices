using IdentityServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IdentityServer.Repositories
{
    public class MongoRepository : IMongoRepository
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;


        public MongoRepository(IConfiguration configuration)
        {

            _client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            _database = _client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

        }

        public IQueryable<T> All<T>() where T : class, new()
        {
            return _database.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression);
        }

        public async void Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var result = await collection.DeleteManyAsync(predicate);

        }
        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression).SingleOrDefault();
        }

        public async Task<bool> CollectionExists<T>() where T : class, new()
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            var totalCount = await collection.CountDocumentsAsync(filter);

            return (totalCount > 0) ? true : false;
        }

        public async void Add<T>(T item) where T : class, new()
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            
            await collection.InsertOneAsync(item);
        }

        public async void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            
            await collection.InsertManyAsync(items);
        }
    }
}
