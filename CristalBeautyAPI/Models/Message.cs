using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Models;

[BsonIgnoreExtraElements]
public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("messageId")]
    public string MessageId { get; set; } = string.Empty;

    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("message")]
    public string MessageText { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = SD.MessageStatus_Unread;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("__v")]
    [BsonIgnoreIfNull]
    public int? Version { get; set; }
}
