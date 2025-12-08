using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace api.Functions;

public class QuotesList
{
    private readonly IQuoteRepository _repo;

    public QuotesList(IQuoteRepository repo)
    {
        _repo = repo;
    }

    [Function("QuotesList")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quotes")] HttpRequestData req)
    {
        var quotes = await _repo.ListAsync();

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(quotes);
        return res;
    }
}
