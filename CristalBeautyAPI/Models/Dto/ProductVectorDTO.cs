using Microsoft.Extensions.VectorData;
using System.ComponentModel.DataAnnotations.Schema;


namespace CristalBeautyAPI.Models.Dto;

public class ProductVectorDTO
{
    [VectorStoreKey]
    public Guid Id { get; set; } = Guid.NewGuid();
    [VectorStoreData]
    public string ProductID { get; set; } = string.Empty;
    [VectorStoreData]
    public string Name { get; set; } = string.Empty;
    [VectorStoreData]
    public double Price { get; set; }
    [VectorStoreData]
    public double LabeledPrice { get; set; }
    [VectorStoreData]
    public string Description { get; set; } = string.Empty;
    [VectorStoreData]
    public string Image { get; set; } = "https://i.insider.com/64381bd0e955f50018fab9d2?width=700&format=jpeg&auto=webp";
    [NotMapped]
    [VectorStoreVector(384, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Vector { get; set; }
}
