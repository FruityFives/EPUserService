

namespace UserServiceAPI.Models
{

    public class UserCreateRequest
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}