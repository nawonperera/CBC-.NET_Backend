using CristalBeautyAPI.Models;
using CristalBeautyAPI.Models.Dto;

namespace CristalBeautyAPI.Services.IServices;

public interface IOrderService
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<List<Order>> GetOrdersByEmailAsync(string email);
    Task CreateOrderAsync(string userEmail, CreateOrderDTO dto);
    Task UpdateOrderAsync(string orderId, UpdateOrderDTO dto);
}
