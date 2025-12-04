using api.Authentication;
using api.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api.Functions;

public class BooksList
{
    private readonly IConfiguration _config;
    private readonly IBookRepository _repo;

    public BooksList(IConfiguration config, IBookRepository repo)
    {
        _config = config;
        _repo = repo;
    }

    [Function("BooksList")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books")] HttpRequestData req)
    {
        var user = JwtReader.GetUser(req, _config, out var error);
        if (user == null)
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync(error);
            return unauth;
        }

        var books = await _repo.ListAsync(user);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(books);
        return res;
    }
}
