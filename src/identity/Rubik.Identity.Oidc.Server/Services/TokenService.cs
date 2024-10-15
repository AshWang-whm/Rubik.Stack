using Microsoft.IdentityModel.Tokens;
using Rubik.Identity.Oidc.Core.Configs;
using Rubik.Identity.Oidc.Core.RsaKey;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Rubik.Identity.Oidc.Core.Services
{
    internal class TokenService(JwkRsaKeys jwkKeys,DiscoveryConfig discovery)
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
                signingCredentials: jwkKeys.SigningCredentials
                );

            var token = jwkKeys.TokenHandler.WriteToken(id_token_options);
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


        public async Task<TokenValidationResult> VerifyAccessToken(string token)
        {
            // 验证token 是否被踢之类的逻辑： todo：


            // 设置要验证的token内容， 可以验证client id之类
            var result = await jwkKeys.TokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer= discovery.Issuer,
                ValidateAudience = false,
                IssuerSigningKey = jwkKeys.RsaSecurityKey
            });

            return result;
        }
    }
}
