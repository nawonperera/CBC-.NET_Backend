namespace CristalBeautyAPI.Models.Dto;

public class CreateProductDTO
{
    public string ProductID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string>? AltNames { get; set; }
    public string Category { get; set; } = "other";
    public double Price { get; set; }
    public double LabeledPrice { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string>? Images { get; set; }
    public int Stock { get; set; }
}
