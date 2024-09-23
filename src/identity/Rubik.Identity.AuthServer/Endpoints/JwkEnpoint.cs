
using Rubik.Identity.Oidc.Core.RsaKey;

namespace Server.Endpoints
{
    public class JwkEnpoint
    {
        public static IResult GetJwks(JwkRsaKeys devKeys)
        {
            return Results.Content(devKeys.JwkJson);
        }

    }


}
