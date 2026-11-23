namespace CristalBeautyAPI.Helpers;

public static class UserHelper
{
    public static Dictionary<string, object?>? GetUser(HttpContext context)
    {
        return context.Items["User"] as Dictionary<string, object?>; //as -> Safe cast: tries to convert the object; returns null if it fails (no exception)
    }

    public static string? GetEmail(Dictionary<string, object?> user)
    {
        return user.TryGetValue("email", out object? val) ? val?.ToString() : null; // Tries to get "email" from the dictionary; returns it as string if found, otherwise null
    }

    public static string? GetRole(Dictionary<string, object?> user)
    {
        return user.TryGetValue("role", out object? val) ? val?.ToString() : null;
    }
}
