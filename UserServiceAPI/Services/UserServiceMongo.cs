using MongoDB.Driver;
using UserServiceAPI.Services;
using UserServiceAPI.Models;
using UserServiceAPI.Repository;

namespace UserServiceAPI.Services
{
    public class UserServiceMongo : IUserServiceMongo
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly ILogger<UserServiceMongo> _logger;

        public UserServiceMongo(ILogger<UserServiceMongo> logger, IConfiguration configuration)
        {
            _logger = logger;
            var connectionString = configuration["MongoDb:ConnectionString"];
            var databaseName = configuration["MongoDb:Database"];
            var collectionName = configuration["MongoDb:Collection"];

            _logger.LogInformation($"Connected to MongoDB using: {connectionString}");
            _logger.LogInformation($"Using database: {databaseName}");
            _logger.LogInformation($"Using collection: {collectionName}");

            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _userCollection = database.GetCollection<User>(collectionName);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to connect to MongoDB: {0}", ex.Message);
                throw;
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

        public async Task<object?> ValidateLogin(Login login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                _logger.LogWarning("Login request is invalid: missing username or password.");
                return null;
            }

            var filter = Builders<User>.Filter.Eq(u => u.Username, login.Username);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning($"User not found: {login.Username}");
                return null;
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning($"Password hash is null or empty for user: {login.Username}");
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);
            if (!isValid)
            {
                _logger.LogWarning($"Invalid password for user: {login.Username}");
                return null;
            }

            _logger.LogInformation($"User {login.Username} successfully validated.");

            // Return kun de nødvendige info – aldrig hele user-objektet med password hash
            return new
            {
                user.UserId,
                user.Username,
                user.EmailAddress,
                user.Role
            };
        }
    }
}