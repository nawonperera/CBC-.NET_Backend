using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Repositories.IRepository;

public interface IOtpRepository
{
    Task InsertAsync(Otp otp);
    Task<Otp?> GetLatestByEmailAsync(string email);
}
