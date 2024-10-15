using IdentityModel;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Rubik.Identity.Oidc.Core;
using System.Text.Json;
namespace Rubik.Identity.AuthServer.Endpoints
{
    public class DiscoveryEndpoint
    {
        static string? _discovertDoc = null;
        public static IResult GetDiscoveryDoc(IServer server)
        {
            if (_discovertDoc == null)
            {
                var config = OidcServer.DiscoveryConfig;
                var address = server.Features.Get<IServerAddressesFeature>()!.Addresses.First();
                var doc = new Dictionary<string, object>
                {
                    { OidcConstants.Discovery.Issuer, config.Issuer },

                    { OidcConstants.Discovery.AuthorizationEndpoint, $"{address}{config.AuthorizationEndpoint}" },
                    { OidcConstants.Discovery.UserInfoEndpoint, $"{address}{config.UserInfoEndpoint}" },
                    { OidcConstants.Discovery.TokenEndpoint, $"{address}{config.TokenEndpoint}" },
                    { OidcConstants.Discovery.DiscoveryEndpoint, $"{address}{config.DiscoveryEndpoint}" },
                    { OidcConstants.Discovery.JwksUri, $"{address}{config.JwksEndpoint}" },
                    { "verifytoken_endpoint", $"{address}{config.VerifyTokenEndpoint}" },

                    { OidcConstants.Discovery.ClaimsSupported, config.Claims },
                    { OidcConstants.Discovery.ScopesSupported, config.Scopes },
                    { OidcConstants.Discovery.ResponseTypesSupported, config.Responsetypes },
                    { OidcConstants.Discovery.SubjectTypesSupported, config.Subjects },
                    { OidcConstants.Discovery.IdTokenSigningAlgorithmsSupported, config.Algorithms }
                };
                _discovertDoc = JsonSerializer.Serialize(doc);
            }

            return Results.Text(_discovertDoc);
        }
    }
}
