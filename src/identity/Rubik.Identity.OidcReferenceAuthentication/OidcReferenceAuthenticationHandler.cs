using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public class OidcReferenceAuthenticationHandler
        (IOptionsMonitor<OidcReferenceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpClientFactory clientFactory,IHttpContextAccessor httpContextAccessor)
                : AuthenticationHandler<OidcReferenceAuthenticationOptions>(options, logger, encoder, clock)
    {
        private readonly HttpClient _httpClient = clientFactory.CreateClient(OidcReferenceDefaults.ReferenceHttpClient);
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 访问远程oidc server，
            // 1.discovery document 获取token verify api
            // 2.默认读取header Bearer key 或 新增MessageReceiveEvent 自定义获取token
            // 3.通过token verify api 验证 token并返回token解析数据
            // 4.token数据写入到IdentityClaims

            var token = httpContextAccessor.HttpContext!.Request.Cookies["access_token"];
            var verify_result = await _httpClient.GetStringAsync(string.Format(Options.VerifyEndpointFormat, token));
            var result = JsonSerializer.Deserialize<TokenVerifyResult>(verify_result);

            if (result?.Result??false)
            {
                var identity = new ClaimsIdentity(Options.Scheme);
                if (Options.SaveClaims)
                {
                    // 解析token
                    var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
                    identity.AddClaims(jwtToken.Payload.Claims);
                }

                // 需要设置authenticationType
                var principal = new ClaimsPrincipal(identity);
                return AuthenticateResult.Success(new AuthenticationTicket(principal, Options.Scheme));
            }

            return AuthenticateResult.Fail(new Exception(result?.Exception ?? "Oidc Reference Token Verify Fail!"));
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
