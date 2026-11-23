using MongoDB.Bson;
using MongoDB.Driver;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services;
using CristalBeautyAPI.Repositories.IRepository;

namespace CristalBeautyAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(MongoDbService db)
    {
        _products = db.Products;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    public async Task<Product?> GetByProductIdAsync(string productId)
    {
        return await _products.Find(p => p.ProductID == productId).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task DeleteByProductIdAsync(string productId)
    {
        await _products.FindOneAndDeleteAsync(p => p.ProductID == productId);
    }

    public async Task UpdateByProductIdAsync(string productId, Product product)
    {
        List<UpdateDefinition<Product>> updateDefs = new();

        if (product.Name != null) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.Name, product.Name));
        }
        if (product.AltNames != null) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.AltNames, product.AltNames));
        }
        if (product.Category != null) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.Category, product.Category));
        }
        if (product.Price != 0) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.Price, product.Price));
        }
        if (product.LabeledPrice != 0) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.LabeledPrice, product.LabeledPrice));
        }
        if (product.Description != null) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.Description, product.Description));
        }
        if (product.Images != null) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.Images, product.Images));
        }
        if (product.Stock != 0) 
        {
            updateDefs.Add(Builders<Product>.Update.Set(p => p.Stock, product.Stock));
        }

        if (updateDefs.Count > 0)
        {
            UpdateDefinition<Product> combinedUpdate = Builders<Product>.Update.Combine(updateDefs);
            await _products.FindOneAndUpdateAsync(p => p.ProductID == productId, combinedUpdate);
        }
    }

    public async Task<List<Product>> FilterAsync(
        string? search, string? category, double? minPrice, double? maxPrice,
        string? sortBy, int skip, int limit)
    {
        FilterDefinition<Product> filter = BuildFilter(search, category, minPrice, maxPrice);
        SortDefinition<Product> sort = BuildSort(sortBy);

        return await _products
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<long> CountFilteredAsync(string? search, string? category, double? minPrice, double? maxPrice)
    {
        FilterDefinition<Product> filter = BuildFilter(search, category, minPrice, maxPrice);
        return await _products.CountDocumentsAsync(filter);
    }

    public async Task<List<string>> GetDistinctCategoriesAsync()
    {
        IAsyncCursor<string> cursor = await _products.DistinctAsync<string>("category", FilterDefinition<Product>.Empty);
        return await cursor.ToListAsync();
    }

    public async Task<List<Product>> SearchAsync(string searchTerm)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Regex("name", new BsonRegularExpression(searchTerm, "i")),
            Builders<Product>.Filter.Regex("altNames", new BsonRegularExpression(searchTerm, "i"))
        );

        return await _products.Find(filter).ToListAsync();
    }

    private FilterDefinition<Product> BuildFilter(string? search, string? category, double? minPrice, double? maxPrice)
    {
        FilterDefinitionBuilder<Product> filterBuilder = Builders<Product>.Filter;
        List<FilterDefinition<Product>> filters = new();

        if (!string.IsNullOrWhiteSpace(search))
        {
            FilterDefinition<Product> searchFilter = filterBuilder.Or(
                filterBuilder.Regex(p => p.Name, new BsonRegularExpression(search, "i")),
                filterBuilder.Regex("altNames", new BsonRegularExpression(search, "i")),
                filterBuilder.Regex(p => p.Description, new BsonRegularExpression(search, "i"))
            );
            filters.Add(searchFilter);
        }

        if (!string.IsNullOrEmpty(category) && category != "all")
        {
            filters.Add(filterBuilder.Eq(p => p.Category, category));
        }

        if (minPrice.HasValue)
        {
            filters.Add(filterBuilder.Gte(p => p.Price, minPrice.Value));
        }

        if (maxPrice.HasValue)
        {
            filters.Add(filterBuilder.Lte(p => p.Price, maxPrice.Value));
        }

        return filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;
    }

    private SortDefinition<Product> BuildSort(string? sortBy)
    {
        return sortBy switch
        {
            "price-low" => Builders<Product>.Sort.Ascending(p => p.Price),
            "price-high" => Builders<Product>.Sort.Descending(p => p.Price),
            "newest" => Builders<Product>.Sort.Descending(p => p.CreatedAt),
            "name-asc" => Builders<Product>.Sort.Ascending(p => p.Name),
            "name-desc" => Builders<Product>.Sort.Descending(p => p.Name),
            _ => Builders<Product>.Sort.Descending(p => p.CreatedAt)
        };
    }
}
