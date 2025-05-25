using UserServiceAPI.Models;

namespace UserServiceAPI.Services;

public interface IUserServiceMongo
{
    Task<User> CreateUser(User user);
    // Task<ValidatedUserResponse?> ValidateLogin(Login login); ss
    Task<object?> ValidateLogin(Login login);
}