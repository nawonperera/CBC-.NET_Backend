using MongoDB.Driver;
using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration configuration)
    {
        string? connectionString = configuration["MongoDb:ConnectionString"];
        string? databaseName = configuration["MongoDb:DatabaseName"];

        MongoClient client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("messages");
    public IMongoCollection<Otp> Otps => _database.GetCollection<Otp>("otps");
}
