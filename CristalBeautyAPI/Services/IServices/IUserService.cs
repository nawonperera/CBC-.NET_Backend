using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Services.IServices;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByEmailAsync(string email);
    Task RegisterUserAsync(string email, string firstName, string lastName, string password, string? phone, string? role);
    Task<(Dictionary<string, object?> userData, string token)> LoginAsync(string email, string password);
    Task<(Dictionary<string, object?> userData, string token)> GoogleLoginAsync(string accessToken);
    Task<string> SendOtpAsync(string email);
    Task ChangePasswordAsync(string email, string password, string otp);
    Task<User?> BlockUserAsync(string email);
    Task<User?> UnblockUserAsync(string email);
    Task<List<string>> GetWishlistAsync(string email);
    Task<List<string>> AddToWishlistAsync(string email, string productId);
    Task<(string action, List<string> wishlist)> ToggleWishlistAsync(string email, string productId);
    Task<List<string>> RemoveFromWishlistAsync(string email, string productId);
}
