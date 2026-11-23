using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Repositories.IRepository;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByProductIdAsync(string productId);
    Task InsertAsync(Product product);
    Task DeleteByProductIdAsync(string productId);
    Task UpdateByProductIdAsync(string productId, Product product);
    Task<List<Product>> FilterAsync(
        string? search, string? category, double? minPrice, double? maxPrice,
        string? sortBy, int skip, int limit);
    Task<long> CountFilteredAsync(string? search, string? category, double? minPrice, double? maxPrice);
    Task<List<string>> GetDistinctCategoriesAsync();
    Task<List<Product>> SearchAsync(string searchTerm);
}
