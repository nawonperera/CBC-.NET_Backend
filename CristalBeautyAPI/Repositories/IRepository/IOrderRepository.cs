using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Repositories.IRepository;

public interface IOrderRepository
{
    Task<List<Order>> GetAllSortedByDateAsync();
    Task<List<Order>> GetByEmailSortedByDateAsync(string email);
    Task<Order?> GetLastOrderAsync();
    Task InsertAsync(Order order);
    Task UpdateAsync(string orderId, Order order);
}
