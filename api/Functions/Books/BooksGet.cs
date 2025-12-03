using api.Authentication;
using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class BooksGet
{
    private readonly IConfiguration _config;
    private readonly IBookRepository _repo;

    public BooksGet(IConfiguration config, IBookRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("BooksGet")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/{id}")] HttpRequestData req,
        string id)
    {
        var user = JwtReader.GetUser(req, _config, out var error);
        if (user == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync(error);
            return unauth;
        }

        var book = await _repo.GetAsync(id, user);
        if (book == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Book not found.");
            return notFound;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(book);
        return res;
    }
}
