using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Models;

[BsonIgnoreExtraElements]
public class BillItem
{
    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;

    [BsonElement("productName")]
    public string ProductName { get; set; } = string.Empty;

    [BsonElement("image")]
    public string Image { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }
}

[BsonIgnoreExtraElements]
public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [BsonElement("date")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = SD.OrderStatus_Pending;

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BsonElement("billItems")]
    public List<BillItem> BillItems { get; set; } = new();

    [BsonElement("total")]
    public double Total { get; set; }

    [BsonElement("__v")]
    [BsonIgnoreIfNull]
    public int? Version { get; set; }
}
