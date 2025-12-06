using api.Authentication;
using api.Models;
using api.Storage;
using api.Sanitization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class BooksUpdate
{
    private readonly IConfiguration _config;
    private readonly IBookRepository _repo;

    public BooksUpdate(IConfiguration config, IBookRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("BooksUpdate")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{id}")] HttpRequestData req,
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
            await notFound.WriteStringAsync("Book not found.");
            return notFound;
        }

        var body = await req.ReadFromJsonAsync<Book>();
        if (body == null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid JSON.");
            return bad;
        }

        // Sanitize fields
        // body.title = InputSanitizer.SanitizeText(body.title);
        // body.author = InputSanitizer.SanitizeText(body.author);

        // If user sent a non-empty title (i.e. actually wants to change it)
        if (!string.IsNullOrWhiteSpace(body.title))
        {
            if (InputSanitizer.IsSanitizedNonEmpty(body.title))
            {
                existing.title = InputSanitizer.SanitizeText(body.title);
            }
            else
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Suggested title input is invalid.");
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

        existing.published = body.published;

        await _repo.UpdateAsync(existing);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(existing);
        return res;
    }
}
