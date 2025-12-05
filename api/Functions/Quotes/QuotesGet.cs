using api.Authentication;
using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class QuotesGet
{
    private readonly IConfiguration _config;
    private readonly IQuoteRepository _repo;

    public QuotesGet(IConfiguration config, IQuoteRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("QuotesGet")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quotes/{id}")] HttpRequestData req,
        string id)
    {
        var user = JwtReader.GetUser(req, _config, out var error);
        if (user == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync(error);
            return unauth;
        }

        var quote = await _repo.GetAsync(id, user);
        if (quote == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Quote not found.");
            return notFound;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(quote);
        return res;
    }
}
