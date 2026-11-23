using Microsoft.AspNetCore.Mvc;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Helpers;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services.IServices;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto)
    {
        Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
        if (currentUser == null) return Unauthorized(new { message = "Unauthorized" });

        try
        {
            string email = UserHelper.GetEmail(currentUser) ?? "";
            await _orderService.CreateOrderAsync(email, dto);
            return Ok(new { message = "Order saved successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Order not saved" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
        if (currentUser == null) return Unauthorized(new { message = "Unauthorized" });

        try
        {
            string? role = UserHelper.GetRole(currentUser);
            string email = UserHelper.GetEmail(currentUser) ?? "";

            List<Order> orders = role == SD.Role_Admin 
                ? await _orderService.GetAllOrdersAsync() 
                : await _orderService.GetOrdersByEmailAsync(email);

            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Orders not found" });
        }
    }

    [HttpPut("{orderId}")]
    public async Task<IActionResult> UpdateOrder(string orderId, [FromBody] UpdateOrderDTO dto)
    {
        Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
        if (currentUser == null) return Unauthorized(new { message = "Unauthorized" });

        try
        {
            await _orderService.UpdateOrderAsync(orderId, dto);
            return Ok(new { message = "Order update Successfully." });
        }
        catch
        {
            return StatusCode(500, new { message = "Order not updated" });
        }
    }
}
