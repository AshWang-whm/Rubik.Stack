using IdentityModel;

namespace Rubik.Identity.AuthServer.Configs
{
    public class DiscoveryConfig
    {
        public string? Issuer { get; set; }
        public string[] Responsetypes {get;set;}= ["code", "token", "id_token"];
        public string[] Claims {get;set;}= ["sid","name","code","dept"];
        public string[] Scopes {get;set;}= ["openid", "profile"];
        public string[] Subjects {get;set;}= ["pairwise", "public"];
        public string[] Algorithms { get; set; } = ["RS256"];

        public string AuthorizationEndpointUrl { get; set; } = "/oauth/authorize";

        public string UserInfoEndpoint { get; set; } = "";

        public string TokenEndpoint { get; set; } = "";

        public string DiscoveryEndpoint { get; set; } = "";

        public string JwksUri { get; set; } = "";
    }
}
