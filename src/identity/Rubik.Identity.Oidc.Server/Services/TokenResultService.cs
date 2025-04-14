using Rubik.Identity.Oidc.Core.Constants;
using Rubik.Identity.Oidc.Core.Dtos;
using Rubik.Identity.Oidc.Core.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace Rubik.Identity.Oidc.Core.Services
{
    internal class TokenResultService(ITokenStore tokenStore, IUserStore userStore, IApiResourceStore apiResourceStore)
    {
        /// <summary>
        /// 生成最终token json结果
        /// </summary>
        /// <param name="oidcQueryParameter"></param>
        /// <param name="tokenJsonParameter"></param>
        /// <returns></returns>
        internal async Task<Dictionary<string,string>> GenerateTokenDictionary(RequestOidcParameterDto oidcQueryParameter, TokenJsonParameterDto tokenJsonParameter)
        {
            // 从scope中区分api scope， 再抓取api scope中是否有用户claims，有则获取用户claims写入access token
            // 从scope中抓取用户claims， 写入id token
            var api_resources = await apiResourceStore.GetApiResources(tokenJsonParameter!.Scope);

            var dict = new Dictionary<string, string>();

            var access_token_claims_types = new List<string?>();
            var id_token_claims_types = new List<string?>();
            if (tokenJsonParameter.IsAccessToken)
            {
                // 抓取用户信息claims, access token的claims type在api scope中
                access_token_claims_types.AddRange(api_resources.SelectMany(a => a.Scopes.Select(s => s.Claims).Where(a => !string.IsNullOrWhiteSpace(a)).SelectMany(a => a!.Split(' ', StringSplitOptions.RemoveEmptyEntries))));
            }
            if (tokenJsonParameter.IsIdToken)
            {
                // id token 的claims type在原始scope中
                id_token_claims_types.AddRange(tokenJsonParameter.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }

            var user_claims_types = access_token_claims_types.Union(id_token_claims_types);
            // 用户信息
            var user_profile_claims = await userStore.GetUserClaims(tokenJsonParameter!.UserCode!, tokenJsonParameter.ClientID, user_claims_types);

            if (tokenJsonParameter.IsAccessToken)
            {
                // access token 默认带上用户账号
                var access_token_claims = new List<Claim>
                {
                    new (JwtRegisteredClaimNames.Sub,tokenJsonParameter.UserCode!),
                };

                if (api_resources.Count > 0)
                {
                    access_token_claims.Add(new Claim(OidcParameterConstants.Scope, string.Join(' ', api_resources.SelectMany(a => a.Scopes.Select(s => s.Scope)))));
                }

                // 用户信息+默认claim = access token
                var api_access_token_claims = user_profile_claims.Where(a => access_token_claims_types.Contains(a.Type));
                access_token_claims.AddRange(api_access_token_claims);

                var access_token = tokenStore.GenerateToken(oidcQueryParameter, access_token_claims);

                dict.Add(OidcParameterConstants.AccessToken, access_token!);

                // refresh token
                if (tokenJsonParameter.ScopeArray?.Contains(OidcParameterConstants.OfflineAccess) ?? false)
                {
                    var refresh_token = TokenGenerateService.GeneratorRefreshToken(access_token!);
                    dict.Add(OidcParameterConstants.RefreshToken, refresh_token);
                }
            }
           

            // 包含openid scope 才输出id token
            if (tokenJsonParameter.IsIdToken)
            {
                // 通过sub & scope 读取用户其他信息 todo：
                var idtoken_claims = new List<Claim>()
                {
                    //iat is required
                    new (JwtRegisteredClaimNames.Iat,DateTime.Now.Ticks.ToString()),
                    new (JwtRegisteredClaimNames.Sub,tokenJsonParameter.UserCode!),
                    new (JwtRegisteredClaimNames.Name,tokenJsonParameter.UserName!),
                };

                // client 端没发送nonce就不需要添加
                // id token 默认需要验证nonce , client端可以配置不验证
                if (!string.IsNullOrEmpty(tokenJsonParameter.Nonce))
                    idtoken_claims.Add(new Claim(JwtRegisteredClaimNames.Nonce, tokenJsonParameter.Nonce));

                // id token
                var scope_id_token_claims = user_profile_claims.Where(a => id_token_claims_types.Contains(a.Type));
                idtoken_claims.AddRange(scope_id_token_claims);

                // id token
                var id_token = tokenStore.GenerateToken(oidcQueryParameter, idtoken_claims);
                dict.Add(OidcParameterConstants.IdToken, id_token!);
            }

            

            return dict;
        }
    }
}
