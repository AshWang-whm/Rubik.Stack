using Microsoft.AspNetCore.Http;
using Rubik.Identity.Oidc.Core.RsaKey;

namespace Rubik.Identity.Oidc.Core.Endpoints
{
    internal class JwkEndpoint
    {
        public static IResult GetJwks(JwkRsaKeys key)
        {
            return Results.Content(key.JwkJson,"application/json");
        }
    }
}
