namespace CristalBeautyAPI.Models.Dto;

public class ChangePasswordDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}
