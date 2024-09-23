using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace Rubik.Identity.Oidc.Core.RsaKey
{
    public class JwkRsaKeys
    {
        public RSA RsaKey { get; }

        public RsaSecurityKey RsaSecurityKey =>new RsaSecurityKey(RsaKey);
        public SigningCredentials SigningCredentials => new SigningCredentials(RsaSecurityKey, SecurityAlgorithms.RsaSha256);
        public JsonWebKey Jwk;
        public JwtSecurityTokenHandler Token_handler = new JwtSecurityTokenHandler();
        public string JwkJson => JsonSerializer.Serialize(new { keys = new List<JsonWebKey> { Jwk } });
        
        public JwkRsaKeys()
        {
            RsaKey = RSA.Create();
            RsaKey.ImportRSAPrivateKey(File.ReadAllBytes(OidcServer.RsaKeyConfig.RsaKeyFileFullPath!), out _);
            Jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(RsaSecurityKey);
            Jwk.Kid = OidcServer.RsaKeyConfig.RsaKid;
        }


    }
}
