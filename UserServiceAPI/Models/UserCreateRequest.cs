using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserServiceAPI.Models
{

    public class UserCreateRequest
    {
        [BsonId]
        [BsonElement("userid")]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}