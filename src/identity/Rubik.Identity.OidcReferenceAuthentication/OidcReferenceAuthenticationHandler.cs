using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Net.Http.Headers;
using System.Web;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public class OidcReferenceAuthenticationHandler
        (IOptionsMonitor<OidcReferenceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpClientFactory clientFactory)
                : AuthenticationHandler<OidcReferenceAuthenticationOptions>(options, logger, encoder, clock)
    {
        private readonly HttpClient _httpClient = clientFactory.CreateClient(OidcReferenceDefaults.ReferenceHttpClient);
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

        protected new OidcReferenceEvents Events
        {
            get => (OidcReferenceEvents)base.Events!;
            set => base.Events = value;
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new OidcReferenceEvents());

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 访问远程oidc server，
            // 1.discovery document 获取token verify api
            // 2.默认读取header Bearer key 或 新增MessageReceiveEvent 自定义获取token
            // 3.通过token verify api 验证 token并返回token解析数据
            // 4.token数据写入到IdentityClaims

            var msgContext = new MessageReceivedContext(Request.HttpContext,Scheme,Options);
            await Events.MessageReceived(msgContext);
            if(msgContext.Result!=null)
                return msgContext.Result;

            string? token = msgContext.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                token = Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization) ? authorization.ToString() : null;
                if (string.IsNullOrWhiteSpace(token))
                {
                    return AuthenticateResult.NoResult();
                }
                if (token?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    token = token["Bearer ".Length..].Trim();
                }
            }


            var url = string.Format(Options.VerifyEndpointRestfulFormat, token);
            var verify_result = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<TokenVerifyResult>(verify_result);

            if (result?.Result??false)
            {
                var identity = new ClaimsIdentity(Options.Scheme);
                if (Options.SaveClaims)
                {
                    // 解析token,考虑缓存处理TODO：
                    var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
                    identity.AddClaims(jwtToken.Payload.Claims);
                }

                // 需要设置authenticationType
                var principal = new ClaimsPrincipal(identity);

                if (Events?.OnTokenValidated != null) 
                {
                    var tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                    {
                        Principal = principal,
                    };
                    await Events.TokenValidated(tokenValidatedContext);

                    if(tokenValidatedContext.Result!=null)
                        return tokenValidatedContext.Result;
                }

                return AuthenticateResult.Success(new AuthenticationTicket(principal, Options.Scheme));
            }

            return AuthenticateResult.Fail(new Exception(result?.Exception ?? "Oidc Reference Token Verify Failed!"));
        }
    }

    public class TokenVerifyResult
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("exception")]
        public string? Exception { get; set; }
    }
}
