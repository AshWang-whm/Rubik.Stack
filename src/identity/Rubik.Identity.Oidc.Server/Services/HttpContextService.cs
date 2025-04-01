using Microsoft.AspNetCore.Http;
using Rubik.Identity.Oidc.Core.Constants;
using Rubik.Identity.Oidc.Core.Dtos;
using Rubik.Identity.Oidc.Core.Exceptions;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using static IdentityModel.OidcConstants;

namespace Rubik.Identity.Oidc.Core.Services
{
    public class HttpContextService(IHttpContextAccessor httpContext)
    {
        internal string? GetQueryParamter(string key)
        {
            httpContext.HttpContext!.Request.Query.TryGetValue(key, out var val);
            return val;
        }

        internal string GetQueryParameterNotNull(string key)
        {
            httpContext.HttpContext!.Request.Query.TryGetValue(key, out var val);
            OidcParameterInValidationException.NotNullOrEmpty(key, val);
            return val!;
        }

        internal string? GetSID()
        {
            return httpContext.HttpContext!.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Sid)?.Value; 
        }

        /// <summary>
        /// 获取Code模式的url 参数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OidcParameterInValidationException"></exception>
        internal AuthorizationEndpointParameter ToCodeQueryParameter()
        {
            var code_challenge = GetQueryParameterNotNull(AuthorizeRequest.CodeChallenge);

            var code_challenge_method = GetQueryParamter(AuthorizeRequest.CodeChallengeMethod);
            var state = GetQueryParameterNotNull(AuthorizeRequest.State);

            var client_id = GetQueryParameterNotNull(AuthorizeRequest.ClientId);

            var scope = GetQueryParameterNotNull(AuthorizeRequest.Scope);


            var nonce = GetQueryParameterNotNull(AuthorizeRequest.Nonce);

            var redirect_uri = GetQueryParameterNotNull(AuthorizeRequest.RedirectUri);

            var response_type = GetQueryParameterNotNull(AuthorizeRequest.ResponseType);

            var parameter= new AuthorizationEndpointParameter
            {
                ClientID = client_id!,
                CodeChallenge = code_challenge!,
                Scope = scope!,
                State = state,
                Nonce = nonce,
                RedirectUri = redirect_uri!,
                CodeChallengeMethod = code_challenge_method,
                ResponseType=response_type
            };

            // 默认从cookies中获取一个用户id
            parameter.UserCode = httpContext.HttpContext!.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Sid)?.Value;


            return parameter;
        }

        /// <summary>
        /// 获取token endpoint 参数,只会从Body获取，Url可以吗？
        /// </summary>
        /// <returns></returns>
        internal async Task<RequestOidcParameterDto> RequestBodyToRequestOidcParameter()
        {
            var body = (await httpContext.HttpContext!.Request.BodyReader.ReadAsync()).Buffer;
            var query = HttpUtility.ParseQueryString(Encoding.UTF8.GetString(body));

            // authorzation_code 和 refresh_token 区分获取token和刷新token
            var grant_type = query.Get(OidcParameterConstants.GrantType);
            var clientid = query.Get(OidcParameterConstants.ClientID);
            
            // 与客户端配置对比，验证客户端密钥，密钥不参与token生成 todo:
            var clientsecret = query.Get(OidcParameterConstants.ClientSecret);

            OidcParameterInValidationException.NotNullOrEmpty(nameof(grant_type), grant_type);

            return new RequestOidcParameterDto
            {
                ClientID = clientid!,
                GrantType = grant_type!,
                ClientSecret = clientsecret,
                Query = query,
            };
        }

        internal RequestOidcParameterDto RequestQueryToRequestOidcParameter()
        {
            var client_id = GetQueryParameterNotNull(AuthorizeRequest.ClientId);
            return new RequestOidcParameterDto
            {
                ClientID= client_id,
                Query = HttpUtility.ParseQueryString(httpContext.HttpContext!.Request.QueryString.Value??""),
            };
        }
    }

    internal class AuthorizationEndpointParameter
    {
        public required string ClientID { get; set; }

        public string? UserCode { get; set; }

        public required string CodeChallenge{ get; set; }

        public required string Scope { get; set; }
        public string[]? ScopeArr => Scope?.Split(' ');

        [JsonIgnore]
        public string? State { get; set; }

        public string? Nonce { get; set; }

        public string? ResponseType { get; set; }

        [JsonIgnore]
        public string? RedirectUri { get; set; }

        [JsonIgnore]
        public string? CodeChallengeMethod { get; set; }

        /// <summary>
        /// 3分钟过期
        /// </summary>
        public DateTime Expire = DateTime.Now.AddMinutes(3);
    }

}
