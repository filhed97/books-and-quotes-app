using api.Authentication;
using api.Models;
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
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

        // override owner + id
        book.OwnerUsername = user;
        book.id ??= Guid.NewGuid().ToString();

        await _repo.AddAsync(book);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(book);
        return res;
    }
}
