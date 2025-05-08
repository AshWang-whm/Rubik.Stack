using Microsoft.AspNetCore.Http;
using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.Dtos;
using Rubik.Identity.Oidc.Core.Extensions;
using Rubik.Identity.Oidc.Core.Services;
using Rubik.Identity.Oidc.Core.Stores;
using System.Web;
using static IdentityModel.OidcConstants;

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

        public static async Task<IResult> Authorize(AuthorizationCodeEncrtptService codeEncrtptService,
            HttpContextService contextService, 
            TokenResultService grantTypeHandleService,
            IClientStore clientStore)
        {
            try
            {
                // 验证
                var response_type = contextService.GetQueryParameterNotNull(AuthorizeRequest.ResponseType);
                var client_id = contextService.GetQueryParameterNotNull(AuthorizeRequest.ClientId);
                var scope = contextService.GetQueryParameterNotNull(AuthorizeRequest.Scope);

                // code => authorization code flow
                // id_token / token => implicit flow
                // code & (id_token / token) => hybrid flow

                // 验证client id
                var client = await clientStore.FindClientByID(client_id!);
                if (client == null)
                    return Results.BadRequest(OidcExceptionConstants.ClientId_Invalid);

                // check scope
                if (Utils.ArrayExcept(client.ResponseType?.Split(' '), response_type.Split(' ')))
                    return Results.BadRequest(OidcExceptionConstants.ResponseType_Invalid);

                var scope_array = scope.Split(' ');
                if (Utils.ArrayExcept(scope_array,client.ScopeArr))
                    return Results.BadRequest(OidcExceptionConstants.Scope_Invalid);

                // 实现 token / id token 的生成
                // code 会跳转到 token endpoint 生成token
                // refresh token 流程在这里还是token endpoint？
                // 非 code 通过GrantTypeHandleService 生成token
                var response_result = response_type switch
                {
                    ResponseTypes.Code => CodeResult(contextService, codeEncrtptService),
                    ResponseTypes.Token or ResponseTypes.IdToken or ResponseTypes.IdTokenToken => await TokenResult(contextService, grantTypeHandleService),
                    _ => Results.BadRequest(TokenErrors.UnsupportedResponseType)
                };
                return response_result;
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
            

        }

        static IResult CodeResult(HttpContextService httpContextService, AuthorizationCodeEncrtptService codeEncrtptService)
        {
            var codeParameter = httpContextService.ToCodeQueryParameter();
            var code = codeEncrtptService.GenerateCode(codeParameter);
            return Results.Redirect($"{codeParameter.RedirectUri}?code={code}&state={codeParameter.State}");
        }

        static async Task<IResult> TokenResult(HttpContextService httpContextService, TokenResultService grantTypeHandleService)
        {
            var redirect_uri = httpContextService.GetQueryParameterNotNull(AuthorizeRequest.RedirectUri);
            var state = httpContextService.GetQueryParameterNotNull(AuthorizeRequest.State);
            var nonce = httpContextService.GetQueryParamter(AuthorizeRequest.Nonce);

            var oidc_parameter = httpContextService.RequestQueryToRequestOidcParameter();

            var token_parameter = new TokenGenDto
            {
                ClientID = oidc_parameter.ClientID,
                Scope = httpContextService.GetQueryParameterNotNull(AuthorizeRequest.Scope),
                UserCode = httpContextService.GetSID(),
                UserName = httpContextService.GetName(),
                Nonce = nonce,
                ResponseType = httpContextService.GetQueryParameterNotNull(AuthorizeRequest.ResponseType)
            };

            var dict = await grantTypeHandleService.GenerateTokenDictionary(oidc_parameter, token_parameter);
            dict.Add(AuthorizeRequest.State, state);
            dict.Add(AuthorizeResponse.TokenType, "Bearer");
            if(!string.IsNullOrWhiteSpace(nonce))
                dict.Add(AuthorizeRequest.Nonce, nonce);

            var query_result = string.Join("&" ,dict.Select(a => $"{a.Key}={HttpUtility.UrlEncode(a.Value)}"));
            string redirectUrl = $"{redirect_uri}?{query_result}";
            var res= Results.Redirect(redirectUrl);
            
            return res;
        }

    }
}
