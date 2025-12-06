using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using System.Net;
using api.Storage;
using api.Sanitization;

namespace api.Authentication;

public class AuthRegister
{
    private readonly IUserRepository _users;

    public AuthRegister(IUserRepository users)
    {
        _users = users;
    }

    [Function("AuthRegister")]
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/register")] HttpRequestData req)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var body = await JsonSerializer.DeserializeAsync<RegisterRequest>(req.Body, options);

        if (body is null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Failed to parse valid request body.");
            return bad;
        }

        // Sanitize fields
        var sanitizedName = InputSanitizer.SanitizeText(body.Username);
        var sanitizedPassword = InputSanitizer.SanitizeText(body.Password);

        if (string.IsNullOrWhiteSpace(sanitizedName))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing or invalid username.");
            return bad;
        }

        if (string.IsNullOrWhiteSpace(sanitizedPassword))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing or invalid password.");
            return bad;
        }

        // call repository with validated username
        await _users.CreateUserAsync(body.Username, body.Password);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { success = true });
        return res;
    }

    public record RegisterRequest(string Username, string Password);
}
