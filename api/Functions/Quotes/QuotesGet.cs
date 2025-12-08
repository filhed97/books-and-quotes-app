using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace api.Functions;

public class QuotesGet
{
    private readonly IQuoteRepository _repo;

    public QuotesGet(IQuoteRepository repo)
    {
        _repo = repo;
    }

    [Function("QuotesGet")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quotes/{id}")] HttpRequestData req,
        string id)
    {
        var quote = await _repo.GetAsync(id);

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
