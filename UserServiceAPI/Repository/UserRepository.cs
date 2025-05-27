using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UserServiceAPI.Models;

namespace UserServiceAPI.Repository
{
    /// <summary>
    /// Repository-klassen til håndtering af brugerdata i MongoDB.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        /// <summary>
        /// Initialiserer en ny instans af UserRepository og konfigurerer MongoDB-forbindelsen.
        /// </summary>
        /// <param name="config">Konfigurationsobjekt, der indeholder MongoDB-indstillinger.</param>
        public UserRepository(IConfiguration config)
        {
            var connectionString = config["MongoDb:ConnectionString"];
            var databaseName = config["MongoDb:Database"];
            var collectionName = config["MongoDb:Collection"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _users = database.GetCollection<User>(collectionName);
        }

        /// <summary>
        /// Opretter en ny bruger i MongoDB-databasen.
        /// </summary>
        /// <param name="user">Brugerobjektet, der skal indsættes.</param>
        /// <returns>Returnerer den oprettede bruger.</returns>
        public async Task<User> CreateUser(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }
    }
}