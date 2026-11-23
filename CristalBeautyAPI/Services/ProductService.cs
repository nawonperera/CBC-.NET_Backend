using CristalBeautyAPI.Models;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Repositories.IRepository;
using CristalBeautyAPI.Services.IServices;

namespace CristalBeautyAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productRepo.GetAllAsync();
    }

    public async Task<Product?> GetProductByIdAsync(string productId)
    {
        return await _productRepo.GetByProductIdAsync(productId);
    }

    public async Task CreateProductAsync(CreateProductDTO dto)
    {
        Product product = new()
        {
            ProductID = dto.ProductID,
            Name = dto.Name,
            AltNames = dto.AltNames ?? new List<string>(),
            Category = dto.Category,
            Price = dto.Price,
            LabeledPrice = dto.LabeledPrice,
            Description = dto.Description,
            Images = dto.Images ?? new List<string> { "https://i.insider.com/64381bd0e955f50018fab9d2?width=700&format=jpeg&auto=webp" },
            Stock = dto.Stock
        };

        await _productRepo.InsertAsync(product);
    }

    public async Task DeleteProductAsync(string productId)
    {
        await _productRepo.DeleteByProductIdAsync(productId);
    }

    public async Task UpdateProductAsync(string productId, UpdateProductDTO dto)
    {
        Product productUpdate = new()
        {
            Name = dto.Name,
            AltNames = dto.AltNames,
            Category = dto.Category,
            Price = dto.Price ?? 0,
            LabeledPrice = dto.LabeledPrice ?? 0,
            Description = dto.Description,
            Images = dto.Images,
            Stock = dto.Stock ?? 0
        };

        await _productRepo.UpdateByProductIdAsync(productId, productUpdate);
    }

    public async Task<(List<Product> products, object pagination)> FilterProductsAsync(
        string? search, string? category, double? minPrice, double? maxPrice,
        string? sortBy, int page, int limit)
    {
        int skip = (page - 1) * limit;
        
        List<Product> products = await _productRepo.FilterAsync(search, category, minPrice, maxPrice, sortBy, skip, limit);

        long totalCount = await _productRepo.CountFilteredAsync(search, category, minPrice, maxPrice);

        object pagination = new
        {
            currentPage = page,
            totalPages = (int)Math.Ceiling((double)totalCount / limit),
            totalProducts = totalCount,
            hasMore = skip + products.Count < totalCount
        };

        return (products, pagination);
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await _productRepo.GetDistinctCategoriesAsync();
    }

    public async Task<List<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _productRepo.SearchAsync(searchTerm);
    }
}
