using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Rubik.Identity.Oidc.Core.Configs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace Rubik.Identity.Oidc.Core.RsaKey
{
    public class JwkRsaKeys
    {
        public RSA RsaKey { get; }

        public RsaSecurityKey RsaSecurityKey =>new(RsaKey);
        public SigningCredentials SigningCredentials => new(RsaSecurityKey, SecurityAlgorithms.RsaSha256);
        public JsonWebKey Jwk;
        public JwtSecurityTokenHandler TokenHandler = new();
        public string JwkJson => JsonSerializer.Serialize(new { keys = new List<JsonWebKey> { Jwk } });
        
        public JwkRsaKeys(RsaKeyConfig keyConfig)
        {
            RsaKey = RSA.Create();
            RsaKey.ImportRSAPrivateKey(File.ReadAllBytes(keyConfig.RsaKeyFileFullPath!), out _);
            Jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(RsaSecurityKey);
            Jwk.Kid = keyConfig.RsaKid;
        }


    }
}
