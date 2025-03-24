using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.Dtos;
using Rubik.Identity.Oidc.Core.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace Rubik.Identity.Oidc.Core.Services
{
    internal class GrantTypeHandleService(ITokenStore tokenStore, IUserStore userStore, IApiResourceStore apiResourceStore)
    {
        /// <summary>
        /// 生成最终token json结果
        /// </summary>
        /// <param name="oidcQueryParameter"></param>
        /// <param name="grantTypeParameter"></param>
        /// <returns></returns>
        public async Task<JsonObject> GrantTypeHandler(OidcQueryParameterDto oidcQueryParameter, GrantTypeHandleDto grantTypeParameter)
        {
            // 从scope中区分api scope， 再抓取api scope中是否有用户claims，有则获取用户claims写入access token
            // 从scope中抓取用户claims， 写入id token
            var api_resources = await apiResourceStore.GetApiResources(grantTypeParameter!.Scope);


            // 抓取用户信息claims, access token的claims type在api scope中
            // id token 的claims type在原始scope中
            var access_token_claims_types = api_resources.SelectMany(a => a.Scopes.Select(s => s.Claims).Where(a => !string.IsNullOrWhiteSpace(a)).SelectMany(a => a!.Split(' ', StringSplitOptions.RemoveEmptyEntries)));
            var id_token_claims_types = grantTypeParameter.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var user_claims_types = access_token_claims_types.Union(id_token_claims_types);
            // 用户信息
            var user_profile_claims = await userStore.GetUserClaims(grantTypeParameter!.UserCode!, grantTypeParameter.ClientID, user_claims_types);

            // access token 默认带上用户账号
            var access_token_claims = new List<Claim>
                {
                    //new(OidcParameterConstant.Scope,auth!.Scope),
                    new (JwtRegisteredClaimNames.Sub,grantTypeParameter.UserCode!),
                };

            if (api_resources.Count > 0)
            {
                access_token_claims.Add(new Claim(OidcParameterConstant.Scope, string.Join(' ', api_resources.SelectMany(a => a.Scopes.Select(s => s.Scope)))));
            }

            // 用户信息+默认claim = access token
            var api_access_token_claims = user_profile_claims.Where(a => access_token_claims_types.Contains(a.Type));
            access_token_claims.AddRange(api_access_token_claims);

            var access_token = tokenStore.GenerateToken(oidcQueryParameter, access_token_claims);
            var json = new JsonObject
            {
                { OidcParameterConstant.AccessToken, access_token }
            };

            // 包含openid scope 才输出id token
            if (grantTypeParameter.ScopeArr?.Contains(OidcParameterConstant.OpenId) ?? false)
            {
                // 通过sub & scope 读取用户其他信息 todo：
                var idtoken_claims = new List<Claim>()
                {
                    //iat is required
                    new (JwtRegisteredClaimNames.Iat,DateTime.Now.Ticks.ToString()),
                };

                // client 端没发送nonce就不需要添加
                // id token 默认需要验证nonce , client端可以配置不验证
                if (!string.IsNullOrEmpty(grantTypeParameter.Nonce))
                    idtoken_claims.Add(new Claim(JwtRegisteredClaimNames.Nonce, grantTypeParameter.Nonce));

                // id token
                var scope_id_token_claims = user_profile_claims.Where(a => id_token_claims_types.Contains(a.Type));
                idtoken_claims.AddRange(scope_id_token_claims);

                // id token
                var id_token = tokenStore.GenerateToken(oidcQueryParameter, idtoken_claims);
                json.Add(OidcParameterConstant.IdToken, id_token);
            }

            // refresh token
            if (grantTypeParameter.ScopeArr?.Contains(OidcParameterConstant.OfflineAccess) ?? false)
            {
                var refresh_token = TokenService.GeneratorRefreshToken(access_token!);
                json.Add(OidcParameterConstant.RefreshToken, refresh_token);
            }

            return json;
        }
    }
}
