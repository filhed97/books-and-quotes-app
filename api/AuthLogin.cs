using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using System.Net;
using api.Storage;

namespace api;

public class AuthLogin
{
    private readonly IUserRepository _users;

    public AuthLogin(IUserRepository users)
    {
        _users = users;
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

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { success = true });
        return res;
    }

    public record LoginRequest(string Username, string Password);
}

