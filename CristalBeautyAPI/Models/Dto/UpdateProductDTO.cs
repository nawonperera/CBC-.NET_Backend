namespace CristalBeautyAPI.Models.Dto;

public class UpdateProductDTO
{
    public string? Name { get; set; }
    public List<string>? AltNames { get; set; }
    public string? Category { get; set; }
    public double? Price { get; set; }
    public double? LabeledPrice { get; set; }
    public string? Description { get; set; }
    public List<string>? Images { get; set; }
    public int? Stock { get; set; }
}
