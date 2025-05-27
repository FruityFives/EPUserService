using UserServiceAPI.Models;
using UserServiceAPI.Services;

namespace UserServiceAPI.Seeding
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(IUserServiceMongo userService, ILogger logger)
        {
            var defaultUsers = new List<UserCreateRequest>
            {
                new UserCreateRequest
                {
                    Username = "henrik",
                    EmailAddress = "admin@example.com",
                    Password = "henrik!",
                    Role = "admin"
                },
                new UserCreateRequest
                {
                    Username = "testuser",
                    EmailAddress = "test@example.com",
                    Password = "Test123!",
                    Role = "user"
                }
            };

            foreach (var user in defaultUsers)
            {
                try
                {
                    await userService.CreateUserFromRequest(user);
                    logger.LogInformation($"Seeded user: {user.Username}");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Could not seed user '{user.Username}': {ex.Message}");
                }
            }
        }
    }
}