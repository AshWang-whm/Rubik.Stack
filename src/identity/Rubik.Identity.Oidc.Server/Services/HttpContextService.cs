using Microsoft.AspNetCore.Http;
using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.Exceptions;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Text;
using System.Web;
using static IdentityModel.OidcConstants;

namespace Rubik.Identity.Oidc.Core.Services
{
    public class HttpContextService(IHttpContextAccessor httpContext)
    {
        public string? GetQueryParamter(string key)
        {
            httpContext.HttpContext!.Request.Query.TryGetValue(key, out var val);
            return val;
        }

        public string GetQueryParameterNotNull(string key)
        {
            httpContext.HttpContext!.Request.Query.TryGetValue(key, out var val);
            OidcParameterInValidationException.NotNullOrEmpty(key, val);
            return val!;
        }

        /// <summary>
        /// 获取Code模式的url 参数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OidcParameterInValidationException"></exception>
        public AuthorizationCodeParameter ToCodeQueryParameter(bool sid = true)
        {
            var code_challenge = GetQueryParameterNotNull(AuthorizeRequest.CodeChallenge);

            var code_challenge_method = GetQueryParameterNotNull(AuthorizeRequest.CodeChallengeMethod);

            var client_id = GetQueryParameterNotNull(AuthorizeRequest.ClientId);

            var scope = GetQueryParameterNotNull(AuthorizeRequest.Scope);

            var state = GetQueryParameterNotNull(AuthorizeRequest.State);

            var nonce = GetQueryParameterNotNull(AuthorizeRequest.Nonce);

            var redirect_uri = GetQueryParameterNotNull(AuthorizeRequest.RedirectUri);

            var parameter= new AuthorizationCodeParameter
            {
                ClientID = client_id!,
                CodeChallenge = code_challenge!,
                Scope = scope!,
                State = state!,
                Nonce = nonce!,
                RedirectUri = redirect_uri!,
                CodeChallengeMethod = code_challenge_method,
            };

            if (sid)
            {
                parameter.UserCode= httpContext.HttpContext!.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Sid)?.Value;
            }

            return parameter;
        }

        /// <summary>
        /// 获取token endpoint 参数
        /// </summary>
        /// <returns></returns>
        public async Task<TokenEndpointParameter> RequestBodyToTokenEndpointParameter()
        {
            var body = (await httpContext.HttpContext!.Request.BodyReader.ReadAsync()).Buffer;
            var query = HttpUtility.ParseQueryString(Encoding.UTF8.GetString(body));

            // authorzation_code 和 refresh_token 区分获取token和刷新token
            var grant_type = query.Get(OidcParameterConstant.GrantType);
            var clientid = query.Get(OidcParameterConstant.ClientID);
            
            // 与客户端配置对比，验证客户端密钥，密钥不参与token生成 todo:
            var clientsecret = query.Get(OidcParameterConstant.ClientSecret);

            OidcParameterInValidationException.NotNullOrEmpty(nameof(grant_type), grant_type);

            return new TokenEndpointParameter
            {
                ClientID = clientid!,
                GrantType = grant_type!,
                ClientSecret = clientsecret,
                Query = query,
            };
        }
    }

    public class AuthorizationCodeParameter
    {
        public required string ClientID { get; set; }
        public string? UserCode { get; set; }
        public required string CodeChallenge{ get; set; }
        public required string Scope { get; set; }
        public string[]? ScopeArr => Scope?.Split(' ');

        public required string State { get; set; }

        public required string Nonce { get; set; }

        public required string RedirectUri { get; set; }

        public string? CodeChallengeMethod { get; set; }

        /// <summary>
        /// 3分钟过期
        /// </summary>
        public DateTime Expire = DateTime.Now.AddMinutes(3);
    }

    public class TokenEndpointParameter
    {
        public required string GrantType { get; set; }

        public required string ClientID { get; set; }

        public string? ClientSecret { get; set; }

        public required NameValueCollection Query { get; set; }
    }
}
