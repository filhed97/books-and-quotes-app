using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Claims;
using api.Authentication;

namespace api.Functions;

public class TestFunction
{
    private readonly IConfiguration _config;

    public TestFunction(IConfiguration config)
    {
        _config = config;
    }

    [Function("TestFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        // Step 1: Read 'Cookie' header
        if (!req.Headers.TryGetValues("Cookie", out var cookieHeaders))
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync("No cookie header.");
            return unauth;
        }

        var cookieHeader = string.Join("; ", cookieHeaders);

        // Step 2: Extract 'auth' token
        var token = ParseCookie(cookieHeader, "auth");
        if (string.IsNullOrEmpty(token))
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync("Auth cookie missing.");
            return unauth;
        }

        // Step 3: Validate JWT
        var jwtKey = _config["JWT_KEY"] ?? throw new InvalidOperationException("JWT_KEY missing");
        var jwtIssuer = _config["JWT_ISSUER"] ?? "your-app";

        var principal = JwtAuthHelpers.ValidateJwt(token, jwtIssuer, jwtKey, out var validationError);
        if (principal == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync("Invalid token: " + validationError);
            return unauth;
        }

        // Step 4: Auth success
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteStringAsync("JWT AUTH SUCCESS");
        return res;
    }

    private static string? ParseCookie(string cookieHeader, string name)
    {
        // cookieHeader format: "a=1; auth=thetoken; b=2"
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
