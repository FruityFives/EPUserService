using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UserServiceAPI.Models;

namespace UserServiceAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            var db = client.GetDatabase(config["MongoDb:Database"]);
            _collection = db.GetCollection<User>("Users");
        }

        public async Task<User> CreateUser(User user)
        {
            await _collection.InsertOneAsync(user);
            return user;
        }
    }
}