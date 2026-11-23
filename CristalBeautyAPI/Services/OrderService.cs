using CristalBeautyAPI.Models;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Repositories.IRepository;
using CristalBeautyAPI.Services.IServices;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;

    public OrderService(IOrderRepository orderRepo, IProductRepository productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepo.GetAllSortedByDateAsync();
    }

    public async Task<List<Order>> GetOrdersByEmailAsync(string email)
    {
        return await _orderRepo.GetByEmailSortedByDateAsync(email);
    }

    public async Task CreateOrderAsync(string userEmail, CreateOrderDTO dto)
    {
        Order? lastOrder = await _orderRepo.GetLastOrderAsync();
        
        string orderId = "ORD001";
        if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.OrderId))
        {
            int lastOrderNumber = int.Parse(lastOrder.OrderId.Replace("ORD", ""));
            orderId = "ORD" + (lastOrderNumber + 1).ToString().PadLeft(4, '0');
        }

        List<BillItem> billItems = new();
        double total = 0;

        foreach (OrderItemDTO item in dto.BillItems)
        {
            Product? product = await _productRepo.GetByProductIdAsync(item.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with product id {item.ProductId} not found.");
            }

            billItems.Add(new BillItem
            {
                ProductId = product.ProductID,
                ProductName = product.Name,
                Image = product.Images.Count > 0 ? product.Images[0] : "",
                Quantity = item.Quantity,
                Price = product.Price
            });

            total += product.Price * item.Quantity;
        }

        Order order = new()
        {
            OrderId = orderId,
            Email = userEmail,
            Name = dto.Name,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            BillItems = billItems,
            Total = total,
            Status = SD.OrderStatus_Pending
        };

        await _orderRepo.InsertAsync(order);
    }

    public async Task UpdateOrderAsync(string orderId, UpdateOrderDTO dto)
    {
        Order orderUpdate = new()
        {
            Status = dto.Status,
            Name = dto.Name,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber
        };

        await _orderRepo.UpdateAsync(orderId, orderUpdate);
    }
}
