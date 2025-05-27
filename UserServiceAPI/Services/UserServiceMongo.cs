using MongoDB.Driver;
using UserServiceAPI.Models;
using UserServiceAPI.Repository;

namespace UserServiceAPI.Services;

/// <summary>
/// Tjenesteklasse til håndtering af forretningslogik relateret til brugere, 
/// herunder oprettelse og validering, med MongoDB som datalager.
/// </summary>
public class UserServiceMongo : IUserServiceMongo
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserServiceMongo> _logger;
    private readonly IMongoCollection<User> _userCollection;

    /// <summary>
    /// Initialiserer en ny instans af <see cref="UserServiceMongo"/> og opsætter forbindelse til MongoDB.
    /// </summary>
    /// <param name="repository">Brugerrepository til databaseoperationer.</param>
    /// <param name="logger">Logger til at logge informationer og fejl.</param>
    /// <param name="configuration">Konfiguration der indeholder MongoDB-indstillinger.</param>
    public UserServiceMongo(IUserRepository repository, ILogger<UserServiceMongo> logger, IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;

        // Henter forbindelsesstrengen fra miljøvariabel eller appsettings
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI")
                               ?? configuration["MongoDb:ConnectionString"];

        var databaseName = configuration["MongoDb:Database"];
        var collectionName = configuration["MongoDb:Collection"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _userCollection = database.GetCollection<User>(collectionName);
    }

    /// <summary>
    /// Opretter en ny bruger og gemmer denne i databasen.
    /// </summary>
    /// <param name="user">Brugerobjektet der skal oprettes.</param>
    /// <returns>Returnerer den oprettede bruger.</returns>
    /// <exception cref="ArgumentNullException">Kastes hvis brugerobjektet er null.</exception>
    /// <exception cref="Exception">Kastes hvis der opstår fejl under databaseoperationen.</exception>
    public async Task<User> CreateUser(User user)
    {
        if (user == null)
        {
            _logger.LogError("CreateUser blev kaldt med null som parameter.");
            throw new ArgumentNullException(nameof(user));
        }

        _logger.LogInformation($"Opretter bruger med ID: {user.UserId}");

        try
        {
            var createdUser = await _repository.AddUser(user);
            _logger.LogInformation($"Bruger {createdUser.UserId} blev oprettet korrekt.");
            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fejl ved oprettelse af bruger {user.UserId}: {ex.Message}");
            throw;
        }
    }

    public async Task<User> CreateUserFromRequest(UserCreateRequest request)
    {
        if (request == null)
        {
            _logger.LogWarning("CreateUserFromRequest blev kaldt med en null request.");
            throw new ArgumentNullException(nameof(request), "Brugerrequest kan ikke være null.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var isValidGuid = Guid.TryParse(request.UserId, out Guid parsedUserId);
        var userId = (!isValidGuid || parsedUserId == Guid.Empty) ? Guid.NewGuid() : parsedUserId;

        var user = new User
        {
            UserId = userId,
            Username = request.Username,
            EmailAddress = request.EmailAddress,
            PasswordHash = hashedPassword,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "user" : request.Role
        };

        return await CreateUser(user);
    }

    /// <summary>
    /// Validerer en brugers loginoplysninger ved at tjekke brugernavn og adgangskode.
    /// </summary>
    /// <param name="login">Login-objekt indeholdende brugernavn og adgangskode.</param>
    /// <returns>
    /// Et anonymt objekt med brugerens ID, brugernavn, e-mail og rolle, 
    /// hvis login er succesfuldt. Returnerer null, hvis login fejler.
    /// </returns>
    public async Task<object?> ValidateLogin(Login login)
    {
        if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
        {
            _logger.LogWarning("Login-request er ugyldig: manglende brugernavn eller adgangskode.");
            return null;
        }

        var filter = Builders<User>.Filter.Eq(u => u.Username, login.Username);
        var user = await _userCollection.Find(filter).FirstOrDefaultAsync();

        if (user == null)
        {
            _logger.LogWarning($"Bruger ikke fundet: {login.Username}");
            return null;
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            _logger.LogWarning($"Adgangskode-hash mangler for bruger: {login.Username}");
            return null;
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);
        if (!isValid)
        {
            _logger.LogWarning($"Ugyldig adgangskode for bruger: {login.Username}");
            return null;
        }

        _logger.LogInformation($"Bruger {login.Username} er blevet valideret korrekt.");

        return new
        {
            user.UserId,
            user.Username,
            user.EmailAddress,
            user.Role
        };

    }
}

