using Microsoft.IdentityModel.Tokens;
using Rubik.Identity.Oidc.Core.Configs;
using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.Dtos;
using Rubik.Identity.Oidc.Core.Extensions;
using Rubik.Identity.Oidc.Core.OidcEntities;
using Rubik.Identity.Oidc.Core.RsaKey;
using Rubik.Identity.Oidc.Core.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Rubik.Identity.Oidc.Core.Services
{
    internal class TokenService(JwkRsaKeys jwkKeys,DiscoveryConfig discovery): ITokenStore
    {
        /// <summary>
        /// 生成 token,算法有限制,HmacSha256 能正常运行
        /// </summary>
        /// <param name="rsaKeys"></param>
        /// <param name="clientid"></param>
        /// <param name="claims"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string? GenerateToken(OidcQueryParameterDto parameter, IEnumerable<Claim>? claims)
        {
            var access_token_options = new JwtSecurityToken(
                issuer: discovery.Issuer,
                audience: parameter.ClientID,
                claims: claims,
                expires: ExpDateByDay(),
                signingCredentials: jwkKeys.SigningCredentials
                );

            var token = jwkKeys.TokenHandler.WriteToken(access_token_options);
            return token;
        }

        public static string GeneratorRefreshToken(string access_token)
        {
            // 简单md5 加密 access_token 当作 refresh token
            return MD5Util.GetMd5Hash(access_token);
        }

        public async Task<RefreshTokenValidationResultEntity> VerifyRefreshToken(OidcQueryParameterDto parameter)
        {
            // 验证md5 签名， refresh token 过期时间默认小于 access_token 过期时间的+3天
            var access_token = parameter.Query[OidcParameterConstant.AccessToken];
            if (access_token == null)
                return new  RefreshTokenValidationResultEntity { IsValid = false, Exception = OidcExceptionConstant.AccessToken_IsRequired };

            var refresh_token = parameter.Query[OidcParameterConstant.RefreshToken];
            if (refresh_token == null)
                return new RefreshTokenValidationResultEntity { IsValid = false, Exception = OidcExceptionConstant.RefreshToken_IsRequired };

            var md5 = MD5Util.GetMd5Hash(access_token);
            if(!md5.Equals(refresh_token))
            {
                return new RefreshTokenValidationResultEntity { IsValid = false, Exception = OidcExceptionConstant.RefreshToken_Invalid };
            }

            // 生成新的access_token 和 refresh token
            var old_access_token= jwkKeys.TokenHandler.ReadJwtToken(access_token);

            var new_access_token = GenerateToken(parameter, old_access_token.Claims);
            var new_refresh_token = GeneratorRefreshToken(new_access_token!);

            return new RefreshTokenValidationResultEntity 
            {
                IsValid = true,
                AccessToken= new_access_token,
                RefreshToken= new_refresh_token,
            };
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


        public DateTime ExpDateByDay(int days=3)
        {
            return DateTime.Now.AddDays(days);
        }


    }
}
