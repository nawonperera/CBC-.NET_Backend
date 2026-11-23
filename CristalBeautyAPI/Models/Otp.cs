using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CristalBeautyAPI.Models;

[BsonIgnoreExtraElements]
public class Otp
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("otp")]
    public string OtpCode { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("__v")]
    [BsonIgnoreIfNull]
    public int? Version { get; set; }
}
