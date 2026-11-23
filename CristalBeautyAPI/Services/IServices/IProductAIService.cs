using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Services.IServices;

public interface IProductAIService
{
    Task<string> SupportAsync(string userQuery);

    //Task UpsertProductAsync(Product product);
    //Task DeleteProductAsync(int productId);
    Task<IEnumerable<Product>> SearchProductsAsync(string query);
}
