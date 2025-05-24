using MongoDB.Driver;
using UserServiceAPI.Services;
using UserServiceAPI.Models;
using UserServiceAPI.Repository;

namespace UserServiceAPI.Services
{
    public class UserServiceMongo :IUserServiceMongo
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly ILogger<UserServiceMongo> _logger;

        public UserServiceMongo(ILogger<UserServiceMongo> logger, IConfiguration configuration)
        {
            _logger = logger;
            var connectionString = configuration["MongoConnectionString"] ?? "<blank>";
            var databaseName = configuration["DatabaseName"] ?? "<blank>";
            var collectionName = configuration["CollectionName"] ?? "<blank>";

            _logger.LogInformation($"Connected to MongoDB using: {connectionString}");
            _logger.LogInformation($" Using database: {databaseName}");
            _logger.LogInformation($" Using Collection: {collectionName}");

            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _userCollection = database.GetCollection<User>(collectionName);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to connect to MongoDB: {0}", ex.Message);
            }
        }

        public async Task<User> CreateUser(User user)
        {
            if (user == null)
            {
                _logger.LogError("CreateUser was called with null user.");
                throw new ArgumentNullException(nameof(user));
            }
            user.UserId = Guid.NewGuid(); // Generér unik ID
            _logger.LogInformation($"Assigning new ID to user: {user.UserId}");

            try
            {
                _logger.LogInformation($"Inserting user {user.UserId} into MongoDB...");
                await _userCollection.InsertOneAsync(user);
                _logger.LogInformation($"User {user.UserId} successfully inserted into MongoDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inserting user {user.UserId} into MongoDB: {ex.Message}");
                throw;
            }

            return user;
        }
        
        }
    }
