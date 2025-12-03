using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace api.Authentication;

public static class JwtReader
{
    public static string? GetUser(
        HttpRequestData req,
        IConfiguration config,
        out string error)
    {
        error = "";

        // 1. Read Cookie header
        if (!req.Headers.TryGetValues("Cookie", out var cookieHeaders))
        {
            error = "Missing Cookie header";
            return null;
        }

        var cookieHeader = string.Join("; ", cookieHeaders);

        // 2. Parse auth cookie
        var token = ParseCookie(cookieHeader, "auth");
        if (string.IsNullOrWhiteSpace(token))
        {
            error = "Missing auth cookie";
            return null;
        }

        // 3. Validate JWT
        var jwtKey = config["JWT_KEY"] ?? throw new InvalidOperationException("JWT_KEY missing");
        var jwtIssuer = config["JWT_ISSUER"] ?? "your-app";

        var principal = JwtAuthHelpers.ValidateJwt(token, jwtIssuer, jwtKey, out var validationError);
        if (principal == null)
        {
            error = "Invalid token: " + validationError;
            return null;
        }

        // 4. Extract username
        var username = principal.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

        if (string.IsNullOrWhiteSpace(username))
        {
            error = "JWT does not contain username claim";
            return null;
        }

        return username;
    }

    private static string? ParseCookie(string cookieHeader, string name)
    {
        var parts = cookieHeader.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var kv = part.Split('=', 2);
            if (kv.Length == 2)
            {
                var key = kv[0].Trim();
                var val = kv[1].Trim();

                if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
                    return val;
            }
        }

        return null;
    }
}
