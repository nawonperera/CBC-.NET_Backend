using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Repositories.IRepository;
using CristalBeautyAPI.Services.IServices;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IOtpRepository _otpRepo;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public UserService(
        IUserRepository userRepo, 
        IOtpRepository otpRepo, 
        IEmailService emailService, 
        IConfiguration config)
    {
        _userRepo = userRepo;
        _otpRepo = otpRepo;
        _emailService = emailService;
        _config = config;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepo.GetAllAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepo.GetByEmailAsync(email);
    }

    public async Task RegisterUserAsync(string email, string firstName, string lastName, string password, string? phone, string? role)
    {
        User? existing = await _userRepo.GetByEmailAsync(email);
        if (existing != null)
        {
            throw new Exception("Email already exists");
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 10);

        User user = new()
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Password = hashedPassword,
            Phone = phone ?? "Not given",
            Role = role ?? SD.Role_User,
            Wishlist = new()
        };

        await _userRepo.InsertAsync(user);
    }

    public async Task<(Dictionary<string, object?> userData, string token)> LoginAsync(string email, string password)
    {
        User? user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid Password");
        }

        Dictionary<string, object?> userData = BuildUserData(user);
        string token = GenerateToken(userData);

        return (userData, token);
    }

    public async Task<(Dictionary<string, object?> userData, string token)> GoogleLoginAsync(string accessToken)
    {
        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        HttpResponseMessage response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Google login failed");
        }

        string content = await response.Content.ReadAsStringAsync();
        JsonElement googleUser = JsonSerializer.Deserialize<JsonElement>(content);

        string email = googleUser.GetProperty("email").GetString()!;
        string givenName = googleUser.GetProperty("given_name").GetString()!;
        string familyName = googleUser.TryGetProperty("family_name", out JsonElement fn) ? fn.GetString() ?? "" : "";

        User? existingUser = await _userRepo.GetByEmailAsync(email);

        if (existingUser == null)
        {
            User newUser = new()
            {
                Email = email,
                FirstName = givenName,
                LastName = familyName,
                IsEmailVerified = true,
                Password = accessToken,
                Wishlist = new()
            };

            await _userRepo.InsertAsync(newUser);
            existingUser = newUser;
        }

        Dictionary<string, object?> userData = BuildUserData(existingUser);
        string token = GenerateToken(userData);

        return (userData, token);
    }

    public async Task<string> SendOtpAsync(string email)
    {
        string otp = new Random().Next(100000, 999999).ToString();

        Otp newOtp = new() { Email = email, OtpCode = otp };
        await _otpRepo.InsertAsync(newOtp);

        await _emailService.SendPlainEmailAsync(
            email, 
            "OPT for email verification", 
            $"Your OTP for email verification is {otp}."
        );

        return otp;
    }

    public async Task ChangePasswordAsync(string email, string password, string otp)
    {
        Otp? lastOtp = await _otpRepo.GetLatestByEmailAsync(email);
        if (lastOtp == null)
        {
            throw new KeyNotFoundException("No OTP found for this email");
        }

        if (lastOtp.OtpCode != otp)
        {
            throw new UnauthorizedAccessException("Invalid OTP");
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 10);
        await _userRepo.UpdatePasswordAsync(email, hashedPassword);
    }

    public async Task<User?> BlockUserAsync(string email)
    {
        return await _userRepo.SetDisabledStatusAsync(email, true);
    }

    public async Task<User?> UnblockUserAsync(string email)
    {
        return await _userRepo.SetDisabledStatusAsync(email, false);
    }

    public async Task<List<string>> GetWishlistAsync(string email)
    {
        User? user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        return user.Wishlist ?? new List<string>();
    }

    public async Task<List<string>> AddToWishlistAsync(string email, string productId)
    {
        if (string.IsNullOrEmpty(productId))
        {
            throw new ArgumentException("Product ID required");
        }
        
        User? user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!user.Wishlist.Contains(productId))
        {
            user.Wishlist.Add(productId);
            await _userRepo.UpdateWishlistAsync(email, user.Wishlist);
        }

        return user.Wishlist;
    }

    public async Task<(string action, List<string> wishlist)> ToggleWishlistAsync(string email, string productId)
    {
        if (string.IsNullOrEmpty(productId))
        {
            throw new ArgumentException("Product ID required");
        }
        User? user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        string action;
        if (user.Wishlist.Contains(productId))
        {
            user.Wishlist.Remove(productId);
            action = "removed";
        }
        else
        {
            user.Wishlist.Add(productId);
            action = "added";
        }

        await _userRepo.UpdateWishlistAsync(email, user.Wishlist);
        return (action, user.Wishlist);
    }

    public async Task<List<string>> RemoveFromWishlistAsync(string email, string productId)
    {
        if (string.IsNullOrEmpty(productId))
        {
            throw new ArgumentException("Product ID required");
        }
        User? user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        user.Wishlist = user.Wishlist.Where(id => id != productId).ToList();
        await _userRepo.UpdateWishlistAsync(email, user.Wishlist);

        return user.Wishlist;
    }

    private Dictionary<string, object?> BuildUserData(User user)
    {
        return new Dictionary<string, object?>
        {
            ["email"] = user.Email,
            ["firstName"] = user.FirstName,
            ["lastName"] = user.LastName,
            ["role"] = user.Role,
            ["phone"] = user.Phone,
            ["isDisabled"] = user.IsDisabled,
            ["isEmailVerifies"] = user.IsEmailVerified
        };
    }

    private string GenerateToken(Dictionary<string, object?> userData)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = userData.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? "")).ToList();

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
