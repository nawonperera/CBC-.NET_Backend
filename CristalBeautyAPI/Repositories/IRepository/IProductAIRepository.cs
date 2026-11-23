namespace CristalBeautyAPI.Repositories.IRepository;

public interface IProductAIRepository
{
    Task<string> SearchProductsAsync(string? keyword = null, decimal? minOrice = null, decimal? maxPrice = null);
}
