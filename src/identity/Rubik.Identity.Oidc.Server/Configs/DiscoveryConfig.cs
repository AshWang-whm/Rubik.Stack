using IdentityModel;

namespace Rubik.Identity.Oidc.Core.Configs
{
    public class DiscoveryConfig
    {
        public string Issuer { get; set; } = "rubik.oidc";
        public string[] Responsetypes {get;set;}= ["code",
        "token",
        "id_token",
        "code token",
        "code id_token",
        "token id_token",
        "code token id_token",];
        public string[] Claims {get;set;}= ["sid","name","code","dept"];
        public string[] Scopes {get;set;}= ["openid", "profile"];
        public string[] Subjects {get;set;}= ["pairwise", "public"];
        public string[] Algorithms { get; set; } = ["RS256"];

        public string[] EncryptionAlgorithms { get; set; } = ["RSA - OAEP"];

        public string[] EncryptionEnc { get; set; } = ["A256GCM"];

        public string AuthorizationEndpoint { get; set; } = "/oauth/authorize";

        public string UserInfoEndpoint { get; set; } = "/oauth/userinfo";

        public string TokenEndpoint { get; set; } = "/oauth/token";

        public string DiscoveryEndpoint { get; set; } = "/.well-known/openid-configuration";
        public string EndSessionEndpoint { get; set; } = "/oauth/logout";
        public string JwksEndpoint { get; set; } = "/oauth/jwks";

        public string VerifyTokenEndpoint { get; set; } = "/oauth/verify";
        public string VerifyTokenRestEndpoint { get; set; } = "/oauth/verify/{token}";
    }
}
