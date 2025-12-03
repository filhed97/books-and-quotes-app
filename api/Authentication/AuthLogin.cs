using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net;
using api.Storage;
using api.Authentication;

namespace api.Authentication;

public class AuthLogin
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public AuthLogin(IUserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    [Function("AuthLogin")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var body = await JsonSerializer.DeserializeAsync<LoginRequest>(req.Body, options);

        if (body is null || string.IsNullOrWhiteSpace(body.Username))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing or invalid username");
            return bad;
        }

        var user = await _users.GetUserAsync(body.Username);

        if (user is null)
        {
            var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorized.WriteStringAsync("User not found");
            return unauthorized;
        }

        // For now: compare plain text
        bool passwordMatches = user.Value.PasswordHash == body.Password;

        if (!passwordMatches)
        {
            var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorized.WriteStringAsync("Invalid password");
            return unauthorized;
        }

        var jwtKey = _config["JWT_KEY"] ?? throw new InvalidOperationException("JWT_KEY missing");
        var jwtIssuer = _config["JWT_ISSUER"] ?? "your-app";

        var token = AuthHelpers.CreateJwt(body.Username, jwtIssuer, jwtKey, TimeSpan.FromHours(8));

        var res = req.CreateResponse(HttpStatusCode.OK);

        // Build cookie string
        var cookie = $"auth={token}; HttpOnly; Path=/; SameSite=Strict; Max-Age={8*3600}; Secure";

        res.Headers.Add("Set-Cookie", cookie);

        await res.WriteAsJsonAsync(new { success = true });
        return res;
    }

    public record LoginRequest(string Username, string Password);
}
