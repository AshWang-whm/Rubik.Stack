using Microsoft.AspNetCore.Http;
using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace Rubik.Identity.Oidc.Core.Endpoints
{
    internal class TokenEndoint
    {
        public static async Task<IResult> GetToken(TokenService tokenService, HttpContextService contextService, AuthorizationCodeEncrtptService codeEncrtptService)
        {
            var parameter =await contextService.RequestBodyToTokenEndpointParameter();

            if (parameter.GrantType == OidcParameterContanst.RefreshToken)
            {
                return await RefreshToken(parameter, tokenService);
            }
            else if (parameter.GrantType == OidcParameterContanst.Authorization_Code)
            {
                return AuthorizationCode(parameter, tokenService,codeEncrtptService);
            }
            else
            {
                // 正常不会走到这个分支 ,  implicit 流程在Authorize中就完成token输出
                // authorization_code 流程会在上面的分支获取token
                // refresh_token 会在第一个分支获取token

                return Results.BadRequest(OidcExceptionContanst.GrantType_NotFound);

                //var access_token_claims = new List<Claim> { };
                //var access_token = tokenService.GeneratorAccessToken(parameter.ClientID, access_token_claims, DateTime.Now.AddMinutes(3));
                //return Results.Json(new
                //{
                //    access_token,
                //    token_type = "Bearer",
                //    expires_in = DateTime.Now.AddSeconds(15),
                //});
            }
        }

        public static async Task<IResult> VerifyReferenceToken(string token,TokenService tokenService)
        {
            var result = await tokenService.VerifyAccessToken(token);
            return Results.Json(new
            {
                Result=result.IsValid,
                Exception=result.IsValid? null: result.Exception.Message
            });
        }

        static async Task<IResult> RefreshToken(TokenEndpointParameter parameter,TokenService tokenService)
        {
            var verify_result =await tokenService.VerifyRefreshToken(parameter);

            if (!verify_result.IsValid)
                return Results.Unauthorized();

            return Results.Json(new
            {
                access_token= verify_result.AccessToken,
                token_type = OidcParameterContanst.Bearer,
                //expires_in = DateTime.Now.AddSeconds(15),
                refresh_token = verify_result.RefreshToken,
            });
        }

        static IResult AuthorizationCode(TokenEndpointParameter parameter, TokenService tokenService, AuthorizationCodeEncrtptService codeEncrtptService)
        {
            // 验证code 以换取token
            var code = parameter.Query.Get(OidcParameterContanst.AuthorizationFlow_Code);
            var code_verifier = parameter.Query.Get(OidcParameterContanst.AuthorizationFlow_Verifier);
            if (!codeEncrtptService.VerifyCode(code, code_verifier, out var auth))
            {
                return Results.BadRequest(OidcExceptionContanst.AuthorizationCode_Invalid);
            }

            // access token 默认带上用户账号
            var access_token_claims = new List<Claim>
                {
                    new(OidcParameterContanst.Scope,auth!.Scope),
                    new (JwtRegisteredClaimNames.Sub,auth.UserCode!),
                };
            var access_token = tokenService.GeneratorAccessToken(parameter, access_token_claims);

            // 通过sid & scope 读取用户其他信息 todo：
            var idtoken_claims = new List<Claim>()
            {
                //sub&iat is required
                new (JwtRegisteredClaimNames.Sub,auth.UserCode!),
                new (JwtRegisteredClaimNames.Iat,DateTime.Now.Ticks.ToString()),
            };

            // client 端没发送nonce就不需要添加
            // id token 默认需要验证nonce , client端可以配置不验证
            if (!string.IsNullOrEmpty(auth.Nonce))
                idtoken_claims.Add(new Claim(JwtRegisteredClaimNames.Nonce, auth.Nonce));

            var json = new JsonObject
            {
                { OidcParameterContanst.AccessToken, access_token }
            };
            // authorization_code 模式返回access_token 和 id_token TODO:

            // id token 会过期吗?
            var id_token = tokenService.GeneratorIdToken(parameter, idtoken_claims);
            json.Add(OidcParameterContanst.IdToken, id_token);

            if(auth.Scope?.Contains(OidcParameterContanst.OfflineAccess) ??false)
            {
                var refresh_token = tokenService.GeneratorRefreshToken(access_token!);
                json.Add(OidcParameterContanst.RefreshToken, refresh_token);
            }
            return Results.Json(json);
        }
    }
}
