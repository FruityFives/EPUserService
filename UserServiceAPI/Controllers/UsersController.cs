using Microsoft.AspNetCore.Mvc;
using UserServiceAPI.Models;
using UserServiceAPI.Services;

namespace UserServiceAPI.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserServiceMongo _service;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserServiceMongo service, ILogger<UsersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Opretter en ny bruger i systemet.
    /// </summary>
    /// <param name="request">Objekt med oplysninger om brugeren, som skal oprettes.</param>
    /// <returns>
    /// Returnerer en 201 Created med den oprettede bruger, hvis det lykkes.
    /// Returnerer 400 Bad Request, hvis anmodningen er ugyldig.
    /// Returnerer 500 Internal Server Error, hvis der opstår en fejl under oprettelsen.
    /// </returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(UserCreateRequest request)
    {
        _logger.LogInformation("Received request to create user.");

        if (request == null)
        {
            _logger.LogWarning("CreateUser was called with a null request.");
            return BadRequest("Request cannot be null.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // ✅ Guid.TryParse til at validere UserId-strengen
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

        var createdUser = await _service.CreateUser(user);

        if (createdUser == null)
        {
            _logger.LogError("Error creating user in database.");
            return StatusCode(500, "Error creating user.");
        }

        _logger.LogInformation($"User created successfully with ID: {createdUser.UserId}");

        return CreatedAtAction(nameof(CreateUser), new { userId = createdUser.UserId }, new
        {
            createdUser.UserId,
            createdUser.Username,
            createdUser.EmailAddress,
            createdUser.Role
        });
    }

    /// <summary>
    /// Validerer brugerens login-oplysninger.
    /// </summary>
    /// <param name="login">Login-objekt med brugernavn og adgangskode.</param>
    /// <returns>
    /// Returnerer 200 OK med brugeroplysninger, hvis login er gyldigt.
    /// Returnerer 401 Unauthorized, hvis loginoplysningerne er ugyldige.
    /// </returns>
    [HttpPost("validate")]
    public async Task<ActionResult<object>> Login([FromBody] Login login)
    {
        var result = await _service.ValidateLogin(login);
        if (result == null)
        {
            return Unauthorized("Invalid credentials");
        }

        return Ok(result);
    }
}
