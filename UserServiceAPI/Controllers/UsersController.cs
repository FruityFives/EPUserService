using Microsoft.AspNetCore.Mvc;
using UserServiceAPI.Models;
using UserServiceAPI.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserServiceAPI.Repository;
using BCrypt.Net;


namespace UserServiceAPI.Controllers;


[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserServiceMongo _service;
    private readonly ILogger<UsersController> _logger;
    private readonly IUserRepository _repository;
    private readonly IUserServiceMongo UserServiceMongo;

    public UsersController(IUserServiceMongo service, ILogger<UsersController> logger, IUserRepository repository)
    {
        _logger = logger;
        _service = service;
        _repository = repository;
    }


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

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            EmailAddress = request.EmailAddress,
            PasswordHash = hashedPassword,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "user" : request.Role
        };

        _logger.LogInformation("Creating user in database...");
        var createdUser = await _service.CreateUser(user);

        if (createdUser == null)
        {
            _logger.LogError("Error creating user in database.");
            return StatusCode(500, "Error creating user.");
        }

        _logger.LogInformation($"User created successfully with ID: {createdUser.UserId}");

        return CreatedAtAction(nameof(CreateUser), new { userId = createdUser.UserId }, createdUser);
    }

    [HttpPost("validate")]
    public async Task<ActionResult<object>> Login([FromBody] Login login)
    {
        var result = await _service.ValidateLogin(login); // Korrekt kald

        if (result == null)
        {
            return Unauthorized("Invalid credentials");
        }

        return Ok(result);
    }


}


    
