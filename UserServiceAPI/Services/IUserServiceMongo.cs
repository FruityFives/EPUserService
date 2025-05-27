using UserServiceAPI.Models;

namespace UserServiceAPI.Services;

public interface IUserServiceMongo
{
    Task<User> CreateUser(User user);
    Task<User> CreateUserFromRequest(UserCreateRequest request);
    Task<object?> ValidateLogin(Login login);
}