using Rubik.Identity.Oidc.Core.Configs;
using Rubik.Identity.Oidc.Core.RsaKey;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Rubik.Identity.Oidc.Core.Services
{
    internal class TokenService(JwkRsaKeys rsaKeys,DiscoveryConfig discovery)
    {
        /// <summary>
        /// 生成 token,算法有限制,HmacSha256 能正常运行
        /// </summary>
        /// <param name="rsaKeys"></param>
        /// <param name="clientid"></param>
        /// <param name="claims"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string? GeneratorAccessToken(string? clientid, List<Claim>? claims, DateTime exp)
        {
            var id_token_options = new JwtSecurityToken(
                issuer: discovery.Issuer,
                audience: clientid,
                claims: claims,
                expires: exp,
                signingCredentials: rsaKeys.SigningCredentials
                );

            var token = rsaKeys.Token_handler.WriteToken(id_token_options);
            return token;
        }

        public string GeneratorRefreshToken()
        {
            return "";
        }

        public string GeneratorIdToken()
        {
            return "";
        }

        public bool VerifyRefreshToken()
        {
            return true;
        }

    }
}
