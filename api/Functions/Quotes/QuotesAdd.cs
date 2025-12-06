using api.Authentication;
using api.Models;
using api.Sanitization;
using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class QuotesAdd
{
    private readonly IConfiguration _config;
    private readonly IQuoteRepository _repo;

    public QuotesAdd(IConfiguration config, IQuoteRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("QuotesAdd")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quotes")] HttpRequestData req)
    {
        var user = JwtReader.GetUser(req, _config, out var error);
        if (user == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync(error);
            return unauth;
        }

        // Ensure user can't have more than 5 quotes
        var quotes = await _repo.ListAsync(user);
        if (quotes.Count >= 5)
        {
            var bres = req.CreateResponse(HttpStatusCode.BadRequest);
            await bres.WriteStringAsync("Maximum of 5 quotes reached.");
            return bres;
        }

        var quote = await req.ReadFromJsonAsync<Quote>();
        if (quote == null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid JSON.");
            return bad;
        }

        // Sanitize fields
        quote.quote = InputSanitizer.SanitizeText(quote.quote);
        quote.author = InputSanitizer.SanitizeText(quote.author);

        if (string.IsNullOrWhiteSpace(quote.quote))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Book title is required and must be valid.");
            return bad;
        }

        if (string.IsNullOrWhiteSpace(quote.author))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Book author is required and must be valid.");
            return bad;
        }

        quote.userId = user;
        quote.id ??= Guid.NewGuid().ToString();

        await _repo.AddAsync(quote);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(quote);
        return res;
    }
}
