using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserServiceAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("UserId")]
        public Guid UserId { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string EmailAddress { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("role")]
        public string? Role { get; set; } = "user";
    }
}