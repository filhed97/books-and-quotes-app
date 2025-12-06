using api.Authentication;
using api.Models;
using api.Sanitization;
using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class BooksAdd
{
    private readonly IConfiguration _config;
    private readonly IBookRepository _repo;

    public BooksAdd(IConfiguration config, IBookRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("BooksAdd")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "books")] HttpRequestData req)
    {
        var user = JwtReader.GetUser(req, _config, out var error);
        if (user == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync(error);
            return unauth;
        }

        var book = await req.ReadFromJsonAsync<Book>();
        if (book == null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid JSON.");
            return bad;
        }

        // Sanitize fields
        book.title = InputSanitizer.SanitizeText(book.title);
        book.author = InputSanitizer.SanitizeText(book.author);

        if (string.IsNullOrWhiteSpace(book.title))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Book title is required and must be valid.");
            return bad;
        }

        if (string.IsNullOrWhiteSpace(book.author))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Book author is required and must be valid.");
            return bad;
        }

        // override owner + id
        book.userId = user;
        book.id ??= Guid.NewGuid().ToString();

        await _repo.AddAsync(book);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(book);
        return res;
    }
}
