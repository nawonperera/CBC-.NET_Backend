using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Repositories.IRepository;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByEmailAsync(string email);
    Task InsertAsync(User user);
    Task UpdatePasswordAsync(string email, string hashedPassword);
    Task<User?> SetDisabledStatusAsync(string email, bool isDisabled);
    Task UpdateWishlistAsync(string email, List<string> wishlist);
}
