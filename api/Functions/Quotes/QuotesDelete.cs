using api.Authentication;
using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class QuotesDelete
{
    private readonly IConfiguration _config;
    private readonly IQuoteRepository _repo;

    public QuotesDelete(IConfiguration config, IQuoteRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("QuotesDelete")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "quotes/{id}")] HttpRequestData req,
        string id)
    {
        // Require logged-in user
        var user = JwtReader.GetUser(req, _config, out var error);
        if (user == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync(error);
            return unauth;
        }

        // Find quote by id (no owner)
        var existing = await _repo.GetAsync(id);
        if (existing == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Quote not found.");
            return notFound;
        }

        // Delete
        await _repo.DeleteAsync(id);

        return req.CreateResponse(HttpStatusCode.OK);
    }
}
