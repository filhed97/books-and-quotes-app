using api.Authentication;
using api.Models;
using api.Sanitization;
using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class QuotesUpdate
{
    private readonly IConfiguration _config;
    private readonly IQuoteRepository _repo;

    public QuotesUpdate(IConfiguration config, IQuoteRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("QuotesUpdate")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "quotes/{id}")] HttpRequestData req,
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

        var body = await req.ReadFromJsonAsync<Quote>();
        if (body == null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid JSON.");
            return bad;
        }

        // Sanitize fields

        // If user sent a non-empty quote (i.e. actually wants to change it)
        if (!string.IsNullOrWhiteSpace(body.quote))
        {
            if (InputSanitizer.IsSanitizedNonEmpty(body.quote))
            {
                existing.quote = InputSanitizer.SanitizeText(body.quote);
            }
            else
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Suggested quote input is invalid.");
                return bad;
            }
        }

        // If user sent a non-empty author
        if (!string.IsNullOrWhiteSpace(body.author))
        {
            if (InputSanitizer.IsSanitizedNonEmpty(body.author))
            {
                existing.author = InputSanitizer.SanitizeText(body.author);
            }
            else
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Suggested author input is invalid.");
                return bad;
            }
        }

        await _repo.UpdateAsync(existing);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(existing);
        return res;
    }
}
