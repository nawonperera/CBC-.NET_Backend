using MongoDB.Driver;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services;
using CristalBeautyAPI.Repositories.IRepository;

namespace CristalBeautyAPI.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(MongoDbService db)
    {
        _orders = db.Orders;
    }

    public async Task<List<Order>> GetAllSortedByDateAsync()
    {
        return await _orders
            .Find(_ => true)
            .SortByDescending(o => o.Date)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByEmailSortedByDateAsync(string email)
    {
        return await _orders
            .Find(o => o.Email == email)
            .SortByDescending(o => o.Date)
            .ToListAsync();
    }

    public async Task<Order?> GetLastOrderAsync()
    {
        List<Order> orders = await _orders
            .Find(_ => true)
            .SortByDescending(o => o.Date)
            .Limit(1)
            .ToListAsync();

        return orders.Count > 0 ? orders[0] : null;
    }

    public async Task InsertAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task UpdateAsync(string orderId, Order order)
    {
        List<UpdateDefinition<Order>> updateDefs = new();

        if (order.Status != null) updateDefs.Add(Builders<Order>.Update.Set(o => o.Status, order.Status));
        if (order.Name != null) updateDefs.Add(Builders<Order>.Update.Set(o => o.Name, order.Name));
        if (order.Address != null) updateDefs.Add(Builders<Order>.Update.Set(o => o.Address, order.Address));
        if (order.PhoneNumber != null) updateDefs.Add(Builders<Order>.Update.Set(o => o.PhoneNumber, order.PhoneNumber));

        if (updateDefs.Count > 0)
        {
            UpdateDefinition<Order> combinedUpdate = Builders<Order>.Update.Combine(updateDefs);
            await _orders.FindOneAndUpdateAsync(o => o.OrderId == orderId, combinedUpdate);
        }
    }
}
