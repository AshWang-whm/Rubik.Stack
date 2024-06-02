using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace Server
{
    public class DevKeys
    {
        public RSA RsaKey { get; }

        public RsaSecurityKey RsaSecurityKey =>new RsaSecurityKey(RsaKey);
        public SigningCredentials SigningCredentials => new SigningCredentials(RsaSecurityKey, SecurityAlgorithms.RsaSha256);
        public JsonWebKey Jwk;
        public JwtSecurityTokenHandler Token_handler = new JwtSecurityTokenHandler();
        public string JwkJson => JsonSerializer.Serialize(new { keys = new List<JsonWebKey> { Jwk } });
        
        public DevKeys(IWebHostEnvironment webHostEnvironment)
        {
            RsaKey = RSA.Create();
            var path = Path.Combine(webHostEnvironment.ContentRootPath,"RsaKey", "crypto_key");
            if(!File.Exists(path))
            {
                throw new Exception("no crypto_key file");
            }
            RsaKey.ImportRSAPrivateKey(File.ReadAllBytes(path), out _);
            Jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(RsaSecurityKey);
            Jwk.Kid = "oidc";
        }


    }
}
