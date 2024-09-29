﻿using Microsoft.AspNetCore.Http;
using Rubik.Identity.Oidc.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Rubik.Identity.Oidc.Core.Endpoints
{
    internal class TokenPoint
    {
        public static async Task<IResult> GetToken(TokenService tokenService, HttpContextService contextService, AuthorizationCodeEncrtptService codeEncrtptService)
        {
            var parameter =await contextService.RequestBodyToTokenEndpointParameter();

            if (parameter.GrantType == "refresh_token")
            {
                var access_token = tokenService.GeneratorRefreshToken();
                return Results.Json(new
                {
                    access_token,
                    token_type = "Bearer",
                    expires_in = DateTime.Now.AddSeconds(15),
                    refresh_token = "refresh_token test",
                });
            }
            else if (parameter.GrantType == "authorization_code")
            {
                // 验证code 以换取token
                var code = parameter.Query.Get("code");
                var code_verifier = parameter.Query.Get("code_verifier");
                if (!codeEncrtptService.VerifyCode(code, code_verifier, out var auth))
                {
                    return Results.BadRequest("invalid code!");
                }


                var access_token_claims = new List<Claim>
                {
                    new Claim("custom_claim","custom_claim_value"),
                    new Claim("scope",auth!.Scope),
                };
                var access_token = tokenService.GeneratorAccessToken(parameter.ClientID, access_token_claims, DateTime.Now.AddMinutes(3));

                var idtoken_claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Sub,"123123"),
                    new Claim(JwtRegisteredClaimNames.Name,"ash"),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.Ticks.ToString()),
                };

                // client 端没发送nonce就不需要添加
                // id token 默认需要验证nonce , client端可以配置不验证
                if (!string.IsNullOrEmpty(auth.Nonce))
                    idtoken_claims.Add(new Claim(JwtRegisteredClaimNames.Nonce, auth.Nonce));

                // id token 会过期吗?
                var id_token = tokenService.GeneratorAccessToken(parameter.ClientID, idtoken_claims, DateTime.Now.AddMinutes(3));
                return Results.Json(new
                {
                    access_token = access_token,
                    token_type = "Bearer",
                    expires_in = DateTime.Now.AddSeconds(15),
                    refresh_token = "refresh_token test",
                    id_token = id_token,
                });
            }
            else
            {
                // 正常不会走到这个分支 ,  implicit 流程在Authorize中就完成token输出
                // authorization_code 流程会在上面的分支获取token
                // refresh_token 会在第一个分支获取token

                var access_token_claims = new List<Claim> { };
                var access_token = tokenService.GeneratorAccessToken(parameter.ClientID, access_token_claims, DateTime.Now.AddMinutes(3));
                return Results.Json(new
                {
                    access_token = access_token,
                    token_type = "Bearer",
                    expires_in = DateTime.Now.AddSeconds(15),
                });
            }
        }



    }
}
