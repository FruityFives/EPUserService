using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UserServiceAPI.Models;

namespace UserServiceAPI.Repository;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    Task<User?> GetUserByUsernameAsync(string username);
}