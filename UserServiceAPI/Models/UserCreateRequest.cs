using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserServiceAPI.Models
{

    public class UserCreateRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }  // MongoDB’s unikke nøgle, kan være automatisk genereret

        [BsonElement("userid")]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }  // Dit “bruger-id” som et separat felt i databasen

        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}