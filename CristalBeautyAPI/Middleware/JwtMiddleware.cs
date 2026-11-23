using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CristalBeautyAPI.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _jwtKey;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _jwtKey = configuration["Jwt:Key"];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? header = context.Request.Headers["Authorization"].FirstOrDefault();

        if (header != null)
        {
            string token = header.Replace("Bearer ", "");

            try
            {
                JwtSecurityTokenHandler tokenHandler = new();
                byte[] key = Encoding.UTF8.GetBytes(_jwtKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;

                // Build a dictionary of user claims
                Dictionary<string, object?> userData = new()
                {
                    ["email"] = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                    ["firstName"] = jwtToken.Claims.FirstOrDefault(c => c.Type == "firstName")?.Value,
                    ["lastName"] = jwtToken.Claims.FirstOrDefault(c => c.Type == "lastName")?.Value,
                    ["role"] = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value,
                    ["phone"] = jwtToken.Claims.FirstOrDefault(c => c.Type == "phone")?.Value,
                    ["isDisabled"] = bool.TryParse(
                        jwtToken.Claims.FirstOrDefault(c => c.Type == "isDisabled")?.Value, out bool disabled) && disabled,
                    ["isEmailVerifies"] = bool.TryParse(
                        jwtToken.Claims.FirstOrDefault(c => c.Type == "isEmailVerifies")?.Value, out bool verified) && verified
                };

                context.Items["User"] = userData;
            }
            catch
            {
                // Invalid token — do nothing
            }
        }

        await _next(context);
    }
}
