using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;

namespace api;

public class AuthLogin
{
    [Function("AuthLogin")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        // Async read of request body
        var requestBody = await req.ReadAsStringAsync();

        // Just echo back the request body for now
        await response.WriteStringAsync(JsonSerializer.Serialize(new { message = "login endpoint hit", body = requestBody }));

        return response;
    }
}
