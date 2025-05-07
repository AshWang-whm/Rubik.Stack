using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Rubik.Identity.Oidc.Core.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Rubik.Identity.Oidc.Core.Authentications
{
    internal class BearerAuthenticationHandler(ITokenStore tokenStore,IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
        : AuthenticationHandler<BearerAuthenticationOptions>(options, logger, encoder)
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            var ifauthorization = Request.Headers.TryGetValue("Authorization", out StringValues authorization);

            if (!ifauthorization)
            {
                return AuthenticateResult.Fail("Authorization Not Found!");
            }
            var token = authorization.ToString().Split("Bearer ").Last();
            var token_valid = await tokenStore.VerifyAccessToken(token);
            if (!token_valid.IsValid)
            {
                return AuthenticateResult.Fail(token_valid.Exception.Message);
            }

            var identity = new ClaimsIdentity(Scheme.Name);
            // 解析token,考虑缓存处理TODO：
            var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
            identity.AddClaims(jwtToken.Payload.Claims);

            // 需要设置authenticationType
            var principal = new ClaimsPrincipal(identity);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
    }
}
