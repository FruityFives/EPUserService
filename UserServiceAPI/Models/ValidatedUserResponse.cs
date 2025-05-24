

namespace UserServiceAPI.Models
{
    public class ValidatedUserResponse
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public User User { get; set; }
    }
}