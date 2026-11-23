using CristalBeautyAPI.Repositories.IRepository;
using CristalBeautyAPI.Services;
using System.ComponentModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace CristalBeautyAPI.Repositories;

public class ProductAIRepository(MongoDbService db) : IProductAIRepository
{
    [Description("Searches the store's product catalog for beauty, cosmetics, and skincare products. Returns up to 10 matching products, or random products if none match.")]
    public async Task<string> SearchProductsAsync(
        [Description("A single singular root keyword to search for (e.g., 'serum', 'cream', 'cleanser', 'sunscreen'). Leave null or empty if the user asks a general beauty or skincare question.")] string? keyword = null,
        [Description("Optional minimum price filter.")] decimal? minPrice = null,
        [Description("Optional maximum price filter.")] decimal? maxPrice = null)
    {
        var query = db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerKeyword));
        }

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= (double)minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= (double)maxPrice.Value);

        var products = await query
            .Take(10)
            .Select(p => new { p.Id, p.Name, p.Price })
            .ToListAsync();

        if (products.Count == 0)
        {
            // Don't declare var products again.
            products = await db.Products.AsQueryable()
                .Take(10)
                .Select(p => new { p.Id, p.Name, p.Price })
                .ToListAsync();
        }

        string productCatalog = products.Count == 0
            ? "- (No products available)"
            : string.Join("\n", products.Select(p => $"- Id:{p.Id} Name:\"{p.Name}\" Price:{p.Price:C}"));

        return productCatalog;
    }

}
