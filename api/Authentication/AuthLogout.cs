using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace api.Authentication;

public class AuthLogout
{
    [Function("AuthLogout")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/logout")] HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);

        // To "delete" the cookie, it's overwritten it with:
        // - Same name
        // - Expired date / Max-Age=0
        // - Same Path
        // - Same SameSite + Secure flags
        var deleteCookie = "auth=; HttpOnly; Path=/; SameSite=Strict; Max-Age=0; Secure";

        res.Headers.Add("Set-Cookie", deleteCookie);

        await res.WriteAsJsonAsync(new { success = true });
        return res;
    }
}
