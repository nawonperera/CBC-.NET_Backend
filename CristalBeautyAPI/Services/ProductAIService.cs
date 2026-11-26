using CristalBeautyAPI.Models;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Repositories.IRepository;
using CristalBeautyAPI.Services.IServices;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace CristalBeautyAPI.Services;

public class ProductAIService(IProductRepository productRepository, IProductAIRepository productAIRepository, IChatClient chatClient, VectorStoreCollection<Guid, ProductVectorDTO> productVectorCollection, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : IProductAIService
{
    public async Task<string> SupportAsync(string userQuery)
    {
        string systemPrompt = """
        You are a helpful assistant for Cristal Beauty Clear, an online beauty, cosmetics, and skincare store.
        Rules:
        1. Only answer questions related to skincare, beauty, cosmetics, or the Cristal Beauty Clear product catalog. For simple greetings, thanks, or farewells, respond politely and briefly without calling tools and without adding a Recommendation line. If otherwise unrelated, do NOT call any tools and say exactly: "I only answer questions about beauty and skincare products."
        2. Be concise and a little funny (light humor).
        3. If you don't know, reply exactly: "I don't know that."
        4. Do not store memory of the conversation.
        5. When appropriate (most user questions), call the SearchProductsAsync tool first to find relevant products, and end your response with ONE relevant product recommendation from the tool results.
           - Pick the product that best matches the user's intent; if none clearly match, pick one of the fallback products returned by the tool.
           - Format the recommendation on a new final line as:
             Recommendation: <Product Name> - LKR:<Price>
        6. Do NOT invent products not returned by the SearchProductsAsync tool.
        """;

        var chatHistory = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userQuery)
        };

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(productAIRepository.SearchProductsAsync)]
        };

        var response = await chatClient.GetResponseAsync(chatHistory, chatOptions);

        return response.Text ?? "No description available.";
    }

    private async Task InitEmbeddingsAsync()
    {
        await productVectorCollection.EnsureCollectionExistsAsync();

        List<Product> products = await productRepository.GetAllAsync();
        foreach (Product product in products)
        {
            string productInfo = $"[{product.Name}] is a product that costs [{product.Price}] and is described as [{product.Description}]";

            ProductVectorDTO productVector = new ProductVectorDTO
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = (double)product.Price,
                Image = product.Images is { Count: > 0 } ? product.Images[0] : string.Empty,
                Vector = await embeddingGenerator.GenerateVectorAsync(productInfo)
            };

            await productVectorCollection.UpsertAsync(productVector);
        }
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        if (!await productVectorCollection.CollectionExistsAsync())
        {
            await InitEmbeddingsAsync();
        }

        var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

        var results = productVectorCollection.SearchAsync(queryEmbedding, 5);

        List<Product> products = [];
        await foreach (var resultItem in results)
        {
            products.Add(new Product
            {
                ProductID = resultItem.Record.ProductID,
                Name = resultItem.Record.Name,
                Description = resultItem.Record.Description,
                Price = (double)resultItem.Record.Price,
                Images = [resultItem.Record.Image],
            });
        }

        return products;
    }
}
