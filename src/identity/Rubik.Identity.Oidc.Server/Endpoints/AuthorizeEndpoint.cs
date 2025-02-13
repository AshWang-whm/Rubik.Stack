using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static IdentityModel.OidcConstants.Algorithms;
using static IdentityModel.OidcConstants;
using Rubik.Identity.Oidc.Core.RsaKey;
using Rubik.Identity.Oidc.Core.Services;
using Rubik.Identity.Oidc.Core.Stores;
using Rubik.Identity.Oidc.Core.Contants;

namespace Rubik.Identity.Oidc.Core.Endpoints
{
    internal class AuthorizeEndpoint
    {
        /// <summary>
        /// form post 类型返回值，0：client callback 1：state 2：id token
        /// </summary>
        const string FORM_POST_FORMAT = @"<html><body onload=""javascript:document.forms[0].submit()""><form method=""post"" action=""{0}""><input type=""hidden"" name=""state"" value=""{1}""/>{2}</form></body></html>";
        const string QUERY_FORMAT = @"{0}?{1}";

        const string QUERY_PARAMETER_FORMAT = @"{0}={1}";
        const string FORM_INPUT_FORMAT = @"<input type=""hidden"" name=""{0}"" value=""{1}""/>";

        public static async Task<IResult> Authorize(AuthorizationCodeEncrtptService codeEncrtptService,HttpContextService contextService, JwkRsaKeys rsaKeys, IClientStore clientStore)
        {
            // 验证
            var response_type = contextService.GetQueryParameterNotNull(AuthorizeRequest.ResponseType);
            // code => authorization code flow
            // id_token / token => implicit flow
            // code & (id_token / token) => hybrid flow

            // 验证scope 等一系列操作 todo:
            var parameter = contextService.ToCodeQueryParameter();

            // 验证client id
            var client = await clientStore.GetClient(parameter.ClientID);
            if (client == null)
                return Results.BadRequest(OidcExceptionConstant.ClientId_Invalid);

            if (client.ResponseType?.Split(' ').Except(response_type.Split(' ')).Any()??true)
                return Results.BadRequest(OidcExceptionConstant.ResponseType_Invalid);

            if (client.ScopeArr?.Except(parameter.ScopeArr ?? []).Any()??true)
                return Results.BadRequest(OidcExceptionConstant.Scope_Invalid);


            var _httpresult = response_type switch
            {
                ResponseTypes.Code => CodeResult(parameter,codeEncrtptService, contextService),
                ResponseTypes.Token => TokenResult(parameter, contextService, rsaKeys),
                ResponseTypes.IdToken => IdTokenResult(parameter, contextService, rsaKeys),
                ResponseTypes.IdTokenToken => IdTokenTokenResult(parameter,contextService, rsaKeys),
                _ => Results.BadRequest(TokenErrors.UnsupportedResponseType)
            };
            return _httpresult;

        }

        static IResult CodeResult(AuthorizationEndpointParameter codeParameter,AuthorizationCodeEncrtptService codeEncrtptService,HttpContextService contextService)
        {
            var code = codeEncrtptService.GenerateCode(codeParameter);
            return Results.Redirect($"{codeParameter.RedirectUri}?code={code}&state={codeParameter.State}");
        }

        /// <summary>
        /// access token 应该包括scope claim，以便访问不同api时做权限控制
        /// claims可以添加一个sub：用户ID
        /// </summary>
        /// <param name="contextService"></param>
        /// <param name="rsaKeys"></param>
        /// <returns></returns>
        static IResult TokenResult(AuthorizationEndpointParameter codeParameter,HttpContextService contextService, JwkRsaKeys rsaKeys)
        {
            var token = Token(codeParameter, contextService, rsaKeys);

            var keys = new List<KeyValuePair<string, string>>
            {
                new(AuthorizeResponse.AccessToken, token),
                new(AuthorizeResponse.TokenType, TokenRequestTypes.Bearer)
            };

            return GenerateResult(codeParameter,contextService, keys);
        }

        /// <summary>
        /// id token 仅包含部分用户信息，包括用户姓名，性别，头像，手机号,角色，权限点等等
        /// 不需要包括 api resource信息，请求api时不需要携带
        /// </summary>
        /// <param name="contextService"></param>
        /// <param name="rsaKeys"></param>
        /// <returns></returns>
        static IResult IdTokenResult(AuthorizationEndpointParameter codeParameter, HttpContextService contextService, JwkRsaKeys rsaKeys)
        {
            var id_token = IdToken(codeParameter, rsaKeys);
            var keys = new List<KeyValuePair<string, string>>
            {
                new( AuthorizeResponse.IdentityToken, id_token)
            };
            return GenerateResult(codeParameter,contextService, keys);
        }

        static IResult IdTokenTokenResult(AuthorizationEndpointParameter parameter,HttpContextService contextService, JwkRsaKeys rsaKeys)
        {
            var token = Token(parameter, contextService, rsaKeys);

            var id_token = IdToken(parameter, rsaKeys, token);
            var keys = new List<KeyValuePair<string, string>>
            {
                new(AuthorizeResponse.AccessToken, token),
                new(AuthorizeResponse.TokenType, TokenRequestTypes.Bearer),
                new( AuthorizeResponse.IdentityToken, id_token)
            };
            return GenerateResult(parameter, contextService, keys);
        }

        static string Token(AuthorizationEndpointParameter parameter, HttpContextService contextService, JwkRsaKeys rsaKeys)
        {
            // 根据scope 获取用户信息： 类似 ApiResource TODO：
            var claims = TokenClaims(parameter);
            return GenerateToken(parameter, claims, rsaKeys);
        }

        /// <summary>
        /// 生成Id Token
        /// </summary>
        /// <param name="contextService"></param>
        /// <param name="rsaKeys"></param>
        /// <returns></returns>
        static string IdToken(AuthorizationEndpointParameter parameter, JwkRsaKeys rsaKeys, string? token = null)
        {
            var claims = IdTokenClaims(parameter);
            if (token != null)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.AtHash, AuthorizationCodeEncrtptService.CreateHashClaimValue(token, Asymmetric.RS256)));
            }
            return GenerateToken(parameter, claims, rsaKeys);
        }

        static List<Claim> TokenClaims(AuthorizationEndpointParameter parameter)
        {
            // 根据scope 获取用户信息： 类似 ApiResource TODO：
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sid,parameter.UserCode!),
                new (OidcParameterConstant.Scope,parameter.Scope),
            };

            return claims;
        }

        static List<Claim> IdTokenClaims(AuthorizationEndpointParameter parameter)
        {
            // 根据scope 获取用户信息： 类似 IdentityResource TODO：
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Iat,DateTime.Now.Ticks.ToString()),
                new(JwtRegisteredClaimNames.Sub,parameter.UserCode!)
            };

            // client 端没发送nonce就不需要添加
            if (!string.IsNullOrEmpty(parameter.Nonce))
                claims.Add(new Claim(JwtRegisteredClaimNames.Nonce, parameter.Nonce!));

            return claims;
        }

        static string GenerateToken(AuthorizationEndpointParameter parameter, List<Claim> claims, JwkRsaKeys rsaKeys)
        {
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, OidcServer.DiscoveryConfig.Issuer));
            var access_token_options = new JwtSecurityToken(
                issuer: OidcServer.DiscoveryConfig.Issuer,
                audience: parameter.ClientID,
                claims: claims,
                expires: parameter.Expire,
                signingCredentials: rsaKeys.SigningCredentials
                );
            var token = rsaKeys.TokenHandler.WriteToken(access_token_options);
            return token;
        }


        /// <summary>
        /// 根据response_mode确定parameter_format, result_format
        /// </summary>
        /// <param name="contextService"></param>
        /// <returns></returns>
        static IResult GenerateResult(AuthorizationEndpointParameter parameter,HttpContextService contextService, IEnumerable<KeyValuePair<string, string>>? inputs)
        {
            if (inputs == null)
                return Results.BadRequest();

            // 不管是query还是form_post ，不是authorization code flow就都是返回一个form html ， 根据id_token和token来判断要哪个token
            // 这个的作用是？？？
            var response_mode = contextService.GetQueryParameterNotNull(AuthorizeRequest.ResponseMode);

            var response_type = contextService.GetQueryParameterNotNull(AuthorizeRequest.ResponseType);

            if (parameter.State.Length == 0)
                return Results.BadRequest();

            bool isquery = response_mode == ResponseModes.Query;
            var parameter_format = isquery ? QUERY_PARAMETER_FORMAT : FORM_INPUT_FORMAT;

            if (isquery)
            {
                var keys = new Dictionary<string, string>(inputs)
                {
                    { AuthorizeResponse.State, parameter.State! }
                };
                var result = string.Join('&', keys.Select(a => $"{a.Key}={a.Value}"));
                var url = string.Format(QUERY_FORMAT, parameter.RedirectUri, result);
                return Results.Redirect(url);
            }
            else
            {
                var body_parameters = string.Join(' ', inputs.Select(a => string.Format(parameter_format, a.Key, a.Value)));

                var body = string.Format(FORM_POST_FORMAT, parameter.RedirectUri, parameter.State, body_parameters);
                return Results.Content(body, "text/html");
            }
        }
    }
}
