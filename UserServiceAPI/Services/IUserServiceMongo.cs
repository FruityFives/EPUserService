using UserServiceAPI.Models;

namespace UserServiceAPI.Services;

public interface IUserServiceMongo
{
    Task<User> CreateUser(User user);
    Task<object?> ValidateLogin(Login login);
}