﻿using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rubik.Identity.Oidc.Core.Constants;
using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.Dtos;
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
        public static async Task<IResult> GetToken([FromServices]ITokenStore tokenService,
            [FromServices] HttpContextService contextService,
            [FromServices] AuthorizationCodeEncrtptService codeEncrtptService,
            //, IClientStore clientStore
            //, IApiResourceStore apiResourceStore
            //, IUserStore userStore,
            [FromServices] TokenResultService grantTypeHandleService)
        {
            var parameter =await contextService.RequestBodyToRequestOidcParameter();

            // 检查scope ？ 

            return parameter.GrantType switch
            {
                OidcParameterConstants.RefreshToken => await RefreshToken(parameter, tokenService),
                OidcParameterConstants.Authorization_Code => await AuthorizationCode(parameter,  codeEncrtptService, grantTypeHandleService),
                // 客户端自行验证用户信息成功后，再向idp申请颁发token？
                // https://oauth.example.com/token?grant_type=client_credentials&client_id=CLIENT_ID&client_secret=CLIENT_SECRET
                //OidcParameterConstant.ClientCredentialsFlow => await ClientCredentialsFlow(parameter, tokenService, clientStore),
                //// 客户端发送用户账号密码&客户端信息到idp，idp验证客户端注册信息&用户信息后，返回token？
                //// https://oauth.example.com/token?grant_type=password&username=USERNAME&password=PASSWORD&client_id=CLIENT_ID
                //OidcParameterConstant.PasswordFlow => await PasswordFlow(parameter, tokenService, clientStore, userStore),
                //OidcParameterConstant.Implicit => await ImplicitFlow(parameter,grantTypeHandleService),
                //OidcParameterConstant.Implicit => ImplicitFlow(parameter, tokenService, clientStore, userStore),
                _ => Results.BadRequest(OidcExceptionConstants.GrantType_IsRequired)
            };
        }

        public static async Task<IResult> VerifyReferenceToken([FromBody]string token,[FromServices]ITokenStore tokenService)
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
        static async Task<IResult> RefreshToken([FromBody] RequestOidcParameterDto parameter,[FromServices]ITokenStore tokenService)
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
                token_type = OidcParameterConstants.Bearer,
                //expires_in = DateTime.Now.AddSeconds(15),
                refresh_token = verify_result.RefreshToken,
            });
        }

        static async Task<IResult> AuthorizationCode(RequestOidcParameterDto parameter
            , AuthorizationCodeEncrtptService codeEncrtptService
            , TokenResultService grantTypeHandleService)
        {
            // 验证code 以换取token
            var code = parameter.Query.Get(OidcParameterConstants.AuthorizationFlow_Code);
            var code_verifier = parameter.Query.Get(OidcParameterConstants.AuthorizationFlow_Verifier);
            if (!codeEncrtptService.VerifyCode(code, code_verifier, out var auth))
            {
                return Results.BadRequest(OidcExceptionConstants.AuthorizationCode_Invalid);
            }

            var granttype_handle_parameter = new TokenJsonParameterDto
            {
                ClientID= auth!.ClientID,
                Scope= auth!.Scope,
                UserCode= auth!.UserCode,
                Nonce= auth!.Nonce,
                ResponseType= auth!.ResponseType!,
            };

            var json = await grantTypeHandleService.GenerateTokenDictionary(parameter, granttype_handle_parameter!);
            return Results.Json(json);
        }


        /// <summary>
        /// password flow does't verify client id ?
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="tokenService"></param>
        /// <param name="clientStore"></param>
        /// <param name="userStore"></param>
        /// <returns></returns>
        static async Task<IResult> PasswordFlow(RequestOidcParameterDto parameter, TokenGenerateService tokenService, IClientStore clientStore,IUserStore userStore)
        {
            var user_id = parameter.Query["username"];
            if (string.IsNullOrWhiteSpace(user_id)) 
            {
                return Results.BadRequest(OidcExceptionConstants.UserName_IsRequired);
            }

            var pwd = parameter.Query["password"];
            if (string.IsNullOrWhiteSpace(pwd))
            {
                return Results.BadRequest(OidcExceptionConstants.Password_IsRequired);
            }

            if(!(await userStore.CheckUser(user_id,pwd)))
            {
                return Results.BadRequest(OidcExceptionConstants.Password_Invalid);
            }


            var access_token_claims = new List<Claim>();
            var scope = parameter.Query["scope"];

            if (!string.IsNullOrWhiteSpace(scope))
                access_token_claims.Add(new(OidcParameterConstants.Scope, scope));

            // 添加 user claims 到 access_token TODO:
            var access_token = tokenService.GenerateToken(parameter, access_token_claims);
            var json = new JsonObject
            {
                { OidcParameterConstants.AccessToken, access_token }
            };

            // refresh token
            if (scope?.Contains(OidcParameterConstants.OfflineAccess)??false)
            {
                // 添加 refresh token
                var refresh_token = TokenGenerateService.GeneratorRefreshToken(access_token!);
                json.Add(OidcParameterConstants.RefreshToken, refresh_token);
            }

            return Results.Json(json);
        }

        static async Task<IResult> ClientCredentialsFlow(RequestOidcParameterDto parameter, TokenGenerateService tokenService, IClientStore clientStore)
        {
            OidcParameterInValidationException.NotNullOrEmpty(nameof(parameter.ClientID), parameter.ClientID);

            var client = await clientStore.GetClient(parameter.ClientID);
            if (client == null || !(client.ClientSecret?.Equals(parameter.ClientSecret) ?? false))
            {
                return Results.BadRequest(OidcExceptionConstants.ClientId_Invalid);
            }

            var access_token_claims = new List<Claim>();
            var scope = parameter.Query["scope"];

            if (!string.IsNullOrWhiteSpace(scope))
                access_token_claims.Add(new(OidcParameterConstants.Scope, scope));

            var access_token = tokenService.GenerateToken(parameter, access_token_claims);
            var json = new JsonObject
            {
                { OidcParameterConstants.AccessToken, access_token }
            };

            return Results.Json(json);
        }
    }
}
