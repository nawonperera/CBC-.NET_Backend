using CristalBeautyAPI.Models;
using CristalBeautyAPI.Models.Dto;

namespace CristalBeautyAPI.Services.IServices;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(string productId);
    Task CreateProductAsync(CreateProductDTO dto);
    Task DeleteProductAsync(string productId);
    Task UpdateProductAsync(string productId, UpdateProductDTO dto);
    Task<(List<Product> products, object pagination)> FilterProductsAsync(
        string? search, string? category, double? minPrice, double? maxPrice,
        string? sortBy, int page, int limit);
    Task<List<string>> GetCategoriesAsync();
    Task<List<Product>> SearchProductsAsync(string searchTerm);
}
