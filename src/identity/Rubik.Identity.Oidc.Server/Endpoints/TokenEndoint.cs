using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.Exceptions;
using Rubik.Identity.Oidc.Core.Services;
using Rubik.Identity.Oidc.Core.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Endpoints
{
    internal class TokenEndoint
    {
        public static async Task<IResult> GetToken(TokenService tokenService, HttpContextService contextService, AuthorizationCodeEncrtptService codeEncrtptService
            ,IClientStore clientStore
            ,IUserStore userStore)
        {
            var parameter =await contextService.RequestBodyToTokenEndpointParameter();


            return parameter.GrantType switch
            {
                OidcParameterConstant.RefreshToken => await RefreshToken(parameter, tokenService),
                OidcParameterConstant.Authorization_Code => await AuthorizationCode(parameter, tokenService, codeEncrtptService, userStore),
                // 客户端自行验证用户信息成功后，再向idp申请颁发token？
                // https://oauth.example.com/token?grant_type=client_credentials&client_id=CLIENT_ID&client_secret=CLIENT_SECRET
                OidcParameterConstant.ClientCredentialsFlow => await ClientCredentialsFlow(parameter, tokenService, clientStore),
                // 客户端发送用户账号密码&客户端信息到idp，idp验证客户端注册信息&用户信息后，返回token？
                // https://oauth.example.com/token?grant_type=password&username=USERNAME&password=PASSWORD&client_id=CLIENT_ID
                OidcParameterConstant.PasswordFlow => await PasswordFlow(parameter, tokenService, clientStore, userStore),
                //OidcParameterConstant.Implicit => ImplicitFlow(parameter, tokenService, clientStore, userStore),
                _ => Results.BadRequest(OidcExceptionConstant.GrantType_IsRequired)
            };
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

        /// <summary>
        /// refresh 的参数是从url或是body获取？
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="tokenService"></param>
        /// <returns></returns>
        static async Task<IResult> RefreshToken(TokenEndpointParameter parameter,TokenService tokenService)
        {
            // query 获取参数
            /*
             https://github.com/login/oauth/access_token?
  client_id=a5ce5a6c7e8c39567ca0&
  client_secret=xxxx&
  redirect_uri=https://coding.net/api/oauth/github/callback&
  grant_type=refresh_token&
  refresh_token=5d633d136b6d56a41829b73a424803ec
             */

            // 默认从body 获取参数

            var verify_result =await tokenService.VerifyRefreshToken(parameter);

            if (!verify_result.IsValid)
                return Results.Unauthorized();

            return Results.Json(new
            {
                access_token= verify_result.AccessToken,
                token_type = OidcParameterConstant.Bearer,
                //expires_in = DateTime.Now.AddSeconds(15),
                refresh_token = verify_result.RefreshToken,
            });
        }

        static async Task<IResult> AuthorizationCode(TokenEndpointParameter parameter, TokenService tokenService, AuthorizationCodeEncrtptService codeEncrtptService, IUserStore userStore)
        {
            // 验证code 以换取token
            var code = parameter.Query.Get(OidcParameterConstant.AuthorizationFlow_Code);
            var code_verifier = parameter.Query.Get(OidcParameterConstant.AuthorizationFlow_Verifier);
            if (!codeEncrtptService.VerifyCode(code, code_verifier, out var auth))
            {
                return Results.BadRequest(OidcExceptionConstant.AuthorizationCode_Invalid);
            }
            // 抓取用户信息claims
            var user_profile_claims = await userStore.MapperUserClaims(auth!.UserCode!,auth.ClientID,auth.Scope);

            // access token 默认带上用户账号
            var access_token_claims = new List<Claim>
                {
                    new(OidcParameterConstant.Scope,auth!.Scope),
                    new (JwtRegisteredClaimNames.Sub,auth.UserCode!),
                    //new(JwtRegisteredClaimNames.Iss,"rubik.oidc")
                };
            // 用户信息+默认claism = access token 
            var access_token = tokenService.GeneratorAccessToken(parameter, access_token_claims.Union(user_profile_claims));

            // 通过sub & scope 读取用户其他信息 todo：
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
                { OidcParameterConstant.AccessToken, access_token }
            };
            // authorization_code 模式返回access_token 和 id_token TODO:

            // id token 会过期吗?
            var id_token = tokenService.GeneratorIdToken(parameter, idtoken_claims);
            json.Add(OidcParameterConstant.IdToken, id_token);

            if(auth.Scope?.Contains(OidcParameterConstant.OfflineAccess) ??false)
            {
                var refresh_token = tokenService.GeneratorRefreshToken(access_token!);
                json.Add(OidcParameterConstant.RefreshToken, refresh_token);
            }
            return Results.Json(json);
        }

        static IResult ImplicitFlow(TokenEndpointParameter parameter, TokenService tokenService, IClientStore clientStore, IUserStore userStore)
        {
            return Results.BadRequest();
        }

        /// <summary>
        /// password flow does't verify client id ?
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="tokenService"></param>
        /// <param name="clientStore"></param>
        /// <param name="userStore"></param>
        /// <returns></returns>
        static async Task<IResult> PasswordFlow(TokenEndpointParameter parameter, TokenService tokenService, IClientStore clientStore,IUserStore userStore)
        {
            var user_id = parameter.Query["username"];
            if (string.IsNullOrWhiteSpace(user_id)) 
            {
                return Results.BadRequest(OidcExceptionConstant.UserName_IsRequired);
            }

            var pwd = parameter.Query["password"];
            if (string.IsNullOrWhiteSpace(pwd))
            {
                return Results.BadRequest(OidcExceptionConstant.Password_IsRequired);
            }

            if(!(await userStore.CheckUser(user_id,pwd)))
            {
                return Results.BadRequest(OidcExceptionConstant.Password_Invalid);
            }


            var access_token_claims = new List<Claim>();
            var scope = parameter.Query["scope"];

            if (!string.IsNullOrWhiteSpace(scope))
                access_token_claims.Add(new(OidcParameterConstant.Scope, scope));

            // 添加 user claims 到 access_token TODO:
            var access_token = tokenService.GeneratorAccessToken(parameter, access_token_claims);
            var json = new JsonObject
            {
                { OidcParameterConstant.AccessToken, access_token }
            };

            // refresh token
            if (scope?.Contains(OidcParameterConstant.OfflineAccess)??false)
            {
                // 添加 refresh token
                var refresh_token = tokenService.GeneratorRefreshToken(access_token!);
                json.Add(OidcParameterConstant.RefreshToken, refresh_token);
            }

            return Results.Json(json);
        }

        static async Task<IResult> ClientCredentialsFlow(TokenEndpointParameter parameter, TokenService tokenService, IClientStore clientStore)
        {
            OidcParameterInValidationException.NotNullOrEmpty(nameof(parameter.ClientID), parameter.ClientID);

            var client = await clientStore.GetClient(parameter.ClientID);
            if (client == null || !(client.ClientSecret?.Equals(parameter.ClientSecret) ?? false))
            {
                return Results.BadRequest(OidcExceptionConstant.ClientId_Invalid);
            }

            var access_token_claims = new List<Claim>();
            var scope = parameter.Query["scope"];

            if (!string.IsNullOrWhiteSpace(scope))
                access_token_claims.Add(new(OidcParameterConstant.Scope, scope));

            var access_token = tokenService.GeneratorAccessToken(parameter, access_token_claims);
            var json = new JsonObject
            {
                { OidcParameterConstant.AccessToken, access_token }
            };

            return Results.Json(json);
        }
    }
}
