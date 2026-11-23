using Microsoft.AspNetCore.Mvc;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Helpers;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services.IServices;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> SaveUser([FromBody] RegisterUserDTO dto)
    {
        if (dto.Role == SD.Role_Admin)
        {
            Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
            if (currentUser == null)
                return StatusCode(403, new { message = "Please login as admin before creating an admin account" });

            if (UserHelper.GetRole(currentUser) != SD.Role_Admin)
                return StatusCode(403, new { message = "You are not authorized to create an admin account" });
        }

        try
        {
            await _userService.RegisterUserAsync(dto.Email, dto.FirstName, dto.LastName, dto.Password, dto.Phone, dto.Role);
            return Ok(new { message = "User saved successfully" });
        }
        catch (Exception ex) when (ex.Message == "Email already exists")
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "User not saved" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            List<User> users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch
        {
            return StatusCode(500, new { message = "users not found" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDTO dto)
    {
        try
        {
            (Dictionary<string, object?> user, string token) = await _userService.LoginAsync(dto.Email, dto.Password);
            return Ok(new { message = "Login Successful", token, user });
        }
        catch (UnauthorizedAccessException ex)
        {
            return ex.Message == "Invalid email" 
                ? NotFound(new { message = ex.Message })
                : StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO dto)
    {
        try
        {
            (Dictionary<string, object?> user, string token) = await _userService.GoogleLoginAsync(dto.AccessToken);
            return Ok(new { message = "Login Successful", token, user });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Google login failed" });
        }
    }

    [HttpGet("current")]
    public IActionResult GetCurrentUser()
    {
        Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
        if (user == null)
            return StatusCode(403, new { message = "please login to get user details" });

        return Ok(new { user });
    }

    [HttpPost("sendMail")]
    public async Task<IActionResult> SendOTP([FromBody] SendOtpDTO dto)
    {
        try
        {
            string otp = await _userService.SendOtpAsync(dto.Email);
            return Ok(new { message = "OTP sent successfully", otp });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to send OTP" });
        }
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        try
        {
            await _userService.ChangePasswordAsync(dto.Email, dto.Password, dto.Otp);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to change password" });
        }
    }

    [HttpPatch("block/{userEmail}")]
    public async Task<IActionResult> BlockUser(string userEmail)
    {
        try
        {
            Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
            if (currentUser == null) return StatusCode(403, new { message = "You need to login first" });
            if (UserHelper.GetRole(currentUser) != SD.Role_Admin) return StatusCode(403, new { message = "Not authorized" });

            User? user = await _userService.BlockUserAsync(userEmail);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(new { message = "User blocked successfully", user });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "User not blocked", error = ex.Message });
        }
    }

    [HttpPatch("unblock/{userEmail}")]
    public async Task<IActionResult> UnblockUser(string userEmail)
    {
        try
        {
            Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
            if (currentUser == null) return StatusCode(403, new { message = "You need to login first" });
            if (UserHelper.GetRole(currentUser) != SD.Role_Admin) return StatusCode(403, new { message = "Not authorized" });

            User? user = await _userService.UnblockUserAsync(userEmail);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(new { message = "User unblocked successfully", user });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "User not unblocked", error = ex.Message });
        }
    }

    [HttpGet("wishlist")]
    public async Task<IActionResult> GetWishlist()
    {
        try
        {
            Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
            if (currentUser == null) return StatusCode(403, new { message = "Please login to view wishlist" });

            string email = UserHelper.GetEmail(currentUser) ?? "";
            List<string> wishlist = await _userService.GetWishlistAsync(email);
            
            return Ok(new { wishlist });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User not found" });
        }
        catch
        {
            return StatusCode(500, new { message = "Failed to fetch wishlist" });
        }
    }

    [HttpPost("wishlist")]
    public async Task<IActionResult> AddToWishlist([FromBody] WishlistDTO dto)
    {
        try
        {
            Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
            if (currentUser == null) return StatusCode(403, new { message = "Please login to add to wishlist" });

            if (string.IsNullOrEmpty(dto.ProductId)) return BadRequest(new { message = "Product ID is required" });

            string email = UserHelper.GetEmail(currentUser) ?? "";
            List<string> wishlist = await _userService.AddToWishlistAsync(email, dto.ProductId);

            return Ok(new { message = "Added to wishlist", wishlist });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User not found" });
        }
        catch
        {
            return StatusCode(500, new { message = "Failed to add to wishlist" });
        }
    }

    [HttpPost("wishlist/toggle")]
    public async Task<IActionResult> ToggleWishlist([FromBody] WishlistDTO dto)
    {
        try
        {
            Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
            if (currentUser == null) return StatusCode(403, new { message = "Please login to manage wishlist" });

            if (string.IsNullOrEmpty(dto.ProductId)) return BadRequest(new { message = "Product ID is required" });

            string email = UserHelper.GetEmail(currentUser) ?? "";
            (string action, List<string> wishlist) result = await _userService.ToggleWishlistAsync(email, dto.ProductId);

            return Ok(new
            {
                message = result.action == "added" ? "Added to wishlist" : "Removed from wishlist",
                result.action,
                wishlist = result.wishlist
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User not found" });
        }
        catch
        {
            return StatusCode(500, new { message = "Failed to update wishlist" });
        }
    }

    [HttpDelete("wishlist/{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(string productId)
    {
        try
        {
            Dictionary<string, object?>? currentUser = UserHelper.GetUser(HttpContext);
            if (currentUser == null) return StatusCode(403, new { message = "Please login to remove from wishlist" });

            if (string.IsNullOrEmpty(productId)) return BadRequest(new { message = "Product ID is required" });

            string email = UserHelper.GetEmail(currentUser) ?? "";
            List<string> wishlist = await _userService.RemoveFromWishlistAsync(email, productId);

            return Ok(new { message = "Removed from wishlist", wishlist });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User not found" });
        }
        catch
        {
            return StatusCode(500, new { message = "Failed to remove from wishlist" });
        }
    }
}
