using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using System.Net;
using api.Storage;

namespace api;

public class AuthRegister
{
    private readonly IUserRepository _users;

    public AuthRegister(IUserRepository users)
    {
        _users = users;
    }

    [Function("AuthRegister")]
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var body = await JsonSerializer.DeserializeAsync<RegisterRequest>(req.Body, options);

        if (body is null || string.IsNullOrWhiteSpace(body.Username))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing or invalid username");
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
