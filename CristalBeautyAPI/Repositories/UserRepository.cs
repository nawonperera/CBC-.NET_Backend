using MongoDB.Driver;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services;
using CristalBeautyAPI.Repositories.IRepository;

namespace CristalBeautyAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoDbService db)
    {
        _users = db.Users;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdatePasswordAsync(string email, string hashedPassword)
    {
        // Build an update instruction (NOT executing anything yet)
        // This creates: { "$set": { "password": hashedPassword } }
        UpdateDefinition<User> update =
            Builders<User>.Update.Set(u => u.Password, hashedPassword);

        // Execute the update in MongoDB
        // Find a document where Email == email
        // Then apply the update instruction (set Password = hashedPassword)
        await _users.UpdateOneAsync(
            u => u.Email == email, // filter (which user to update)
            update                 // update instruction (what to change)
        );
    }

    public async Task<User?> SetDisabledStatusAsync(string email, bool isDisabled)
    {
        UpdateDefinition<User> update = Builders<User>.Update.Set(u => u.IsDisabled, isDisabled);
        return await _users.FindOneAndUpdateAsync(
            u => u.Email == email,
            update,
            new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After });
    }

    public async Task UpdateWishlistAsync(string email, List<string> wishlist)
    {
        UpdateDefinition<User> update = Builders<User>.Update.Set(u => u.Wishlist, wishlist);
        await _users.UpdateOneAsync(u => u.Email == email, update);
    }
}
