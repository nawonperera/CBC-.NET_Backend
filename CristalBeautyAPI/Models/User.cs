using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Models;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = SD.Role_User;

    [BsonElement("password")]
    public string Password { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = "Not given";

    [BsonElement("isDisabled")]
    public bool IsDisabled { get; set; } = false;

    [BsonElement("isEmailVerified")]
    public bool IsEmailVerified { get; set; } = false;

    [BsonElement("wishlist")]
    public List<string> Wishlist { get; set; } = new();

    // [BsonElement("__v")]  // Maps this property to the "__v" field in the MongoDB document
    // [BsonIgnoreIfNull]
    // public int? Version { get; set; }
}
