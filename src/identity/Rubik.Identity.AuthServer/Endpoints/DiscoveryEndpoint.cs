using IdentityModel;
namespace Rubik.Identity.AuthServer.Endpoints
{
    public class DiscoveryEndpoint
    {
        private static readonly string[] responsetypes = ["code", "token", "id_token"];
        private static readonly string[] claims = ["sid", "email", "name"];
        private static readonly string[] scopes = ["openid", "profile", "api1", "api2", "client1", "client2"];
        private static readonly string[] subjects = ["pairwise", "public"];
        private static readonly string[] algorithms = ["RS256"];

        public static IResult GetDiscoveryDoc(HttpContext httpContext)
        {
            var doc = new Dictionary<string, object>
            {
                { OidcConstants.Discovery.Issuer, "ash.oauth" },

                { OidcConstants.Discovery.AuthorizationEndpoint, "http://localhost:5021/oauth/authorize" },
                { OidcConstants.Discovery.UserInfoEndpoint, "http://localhost:5021/oauth/userinfo" },
                { OidcConstants.Discovery.TokenEndpoint, "http://localhost:5021/oauth/token" },
                { OidcConstants.Discovery.DiscoveryEndpoint, "http://localhost:5021/.well-known/openid-configuration" },
                { OidcConstants.Discovery.JwksUri, "http://localhost:5021/oauth/jwks" },

                { OidcConstants.Discovery.ClaimsSupported, claims },
                { OidcConstants.Discovery.ScopesSupported, scopes },
                { OidcConstants.Discovery.ResponseTypesSupported, responsetypes },
                { OidcConstants.Discovery.SubjectTypesSupported, subjects },
                { OidcConstants.Discovery.IdTokenSigningAlgorithmsSupported, algorithms }

            };

            return Results.Json(doc);
        }

    }
}
