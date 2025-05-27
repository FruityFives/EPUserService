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
            return BadRequest("Request cannot be null.");

        try
        {
            var createdUser = await _service.CreateUserFromRequest(request);

            return CreatedAtAction(nameof(CreateUser), new { userId = createdUser.UserId }, new
            {
                createdUser.UserId,
                createdUser.Username,
                createdUser.EmailAddress,
                createdUser.Role
            });
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning(ex, "Invalid request.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user.");
            return StatusCode(500, "Internal server error.");
        }
    }
    
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

