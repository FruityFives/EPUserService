using UserServiceAPI.Models;
using UserServiceAPI.Services;

namespace UserServiceAPI.SeedData
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(IUserServiceMongo userService, ILogger logger, int maxRetries = 5, int delaySeconds = 3)
        {
            var defaultUsers = new List<UserCreateRequest>
            {
                new UserCreateRequest
                {
                    Username = "henrik",
                    EmailAddress = "admin@example.com",
                    Password = "henrik",
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

            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    foreach (var user in defaultUsers)
                    {
                        await userService.CreateUserFromRequest(user);
                        logger.LogInformation($"Seeded user: {user.Username}");
                    }
                    // Success: exit retry loop
                    return;
                }
                catch (Exception ex)
                {
                    attempt++;
                    logger.LogWarning($"Attempt {attempt} to seed users failed: {ex.Message}");
                    if (attempt == maxRetries)
                    {
                        logger.LogError("Max retry attempts reached. Could not seed users.");
                        throw; // or just return to continue silently
                    }
                    await Task.Delay(delaySeconds * 10);
                }
            }
        }
    }
}