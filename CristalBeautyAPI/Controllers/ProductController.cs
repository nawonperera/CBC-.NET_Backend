using Microsoft.AspNetCore.Mvc;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Helpers;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services.IServices;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO dto)
    {
        Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
        if (user == null)
        {
            return StatusCode(403, new { message = "You need to login first" });
        }

        if (UserHelper.GetRole(user) != SD.Role_Admin)
        {
            return StatusCode(403, new { message = "You are not authorized to create a product" });
        }

        try
        {
            await _productService.CreateProductAsync(dto);
            return Ok(new { message = "Product saved successfully" });
        }
        catch
        {
            return StatusCode(500, new { message = "Product not saved." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            List<Product> products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch
        {
            return StatusCode(500, new { message = "Product not found" });
        }
    }

    [HttpGet("filter")]
    public async Task<IActionResult> FilterProducts(
        [FromQuery] string? search, [FromQuery] string? category,
        [FromQuery] double? minPrice, [FromQuery] double? maxPrice,
        [FromQuery] string? sortBy, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        try
        {
            (List<Product> products, object pagination) result = await _productService.FilterProductsAsync(search, category, minPrice, maxPrice, sortBy, page, limit);
            return Ok(new
            {
                products = result.products,
                pagination = result.pagination
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error filtering products" });
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            List<string> categories = await _productService.GetCategoriesAsync();
            return Ok(new { categories });
        }
        catch
        {
            return StatusCode(500, new { message = "Error fetching categories" });
        }
    }

    [HttpGet("search/{id}")]
    public async Task<IActionResult> SearchProducts(string id)
    {
        try
        {
            List<Product> products = await _productService.SearchProductsAsync(id);
            return Ok(new { products });
        }
        catch
        {
            return StatusCode(500, new { message = "Error searching products" });
        }
    }

    [HttpDelete("{productID}")]
    public async Task<IActionResult> DeleteProduct(string productID)
    {
        Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
        if (user == null) return StatusCode(403, new { message = "You need to login first" });
        if (UserHelper.GetRole(user) != SD.Role_Admin) return StatusCode(403, new { message = "You are not authorized to create a product" });

        try
        {
            await _productService.DeleteProductAsync(productID);
            return Ok(new { message = "Product deleted successfully" });
        }
        catch
        {
            return StatusCode(500, new { message = "Product not deleted" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(string id)
    {
        Product? product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound(new { message = "Product not found." });

        return Ok(new { product });
    }

    [HttpPut("{productID}")]
    public async Task<IActionResult> UpdateProduct(string productID, [FromBody] UpdateProductDTO dto)
    {
        Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
        if (user == null) return StatusCode(403, new { message = "You need to login first" });
        if (UserHelper.GetRole(user) != SD.Role_Admin) return StatusCode(403, new { message = "You are not authorized to create a product" });

        try
        {
            await _productService.UpdateProductAsync(productID, dto);
            return Ok(new { message = "Product updated successfully" });
        }
        catch
        {
            return StatusCode(500, new { message = "Product not updated" });
        }
    }
}
