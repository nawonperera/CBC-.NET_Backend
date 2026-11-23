using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CristalBeautyAPI.Models;

[BsonIgnoreExtraElements]
public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("productID")]
    public string ProductID { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("altNames")]
    public List<string> AltNames { get; set; } = new();

    [BsonElement("category")]
    public string Category { get; set; } = "other";

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("labeledPrice")]
    public double LabeledPrice { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("images")]
    public List<string> Images { get; set; } = new()
    {
        "https://i.insider.com/64381bd0e955f50018fab9d2?width=700&format=jpeg&auto=webp"
    };

    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("__v")]
    [BsonIgnoreIfNull]
    public int? Version { get; set; }
}
