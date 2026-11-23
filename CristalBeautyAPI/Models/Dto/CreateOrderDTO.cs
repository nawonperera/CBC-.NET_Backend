namespace CristalBeautyAPI.Models.Dto;

public class CreateOrderDTO
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<OrderItemDTO> BillItems { get; set; } = new();
}

public class OrderItemDTO
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
