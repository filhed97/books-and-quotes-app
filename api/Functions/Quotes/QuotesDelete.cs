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
        var user = JwtReader.GetUser(req, _config, out var error);
        if (user == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync(error);
            return unauth;
        }

        var existing = await _repo.GetAsync(id, user);
        if (existing == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Quote not found.");
            return notFound;
        }

        await _repo.DeleteAsync(id, user);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteStringAsync("Deleted");
        return res;
    }
}
