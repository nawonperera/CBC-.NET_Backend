using MongoDB.Driver;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services;
using CristalBeautyAPI.Repositories.IRepository;

namespace CristalBeautyAPI.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly IMongoCollection<Otp> _otps;

    public OtpRepository(MongoDbService db)
    {
        _otps = db.Otps;
    }

    public async Task InsertAsync(Otp otp)
    {
        await _otps.InsertOneAsync(otp);
    }

    public async Task<Otp?> GetLatestByEmailAsync(string email)
    {
        return await _otps
            .Find(o => o.Email == email)
            .SortByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
