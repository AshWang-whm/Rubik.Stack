using Microsoft.IdentityModel.Tokens;
using Rubik.Identity.Oidc.Core.Attributes;
using Rubik.Identity.Oidc.Core.Dtos;
using Rubik.Identity.Oidc.Core.OidcEntities;
using System.Security.Claims;

namespace Rubik.Identity.Oidc.Core.Stores
{
    
    [AutoInject(AutoInjectType.Scope)]
    public interface ITokenStore
    {
        public string? GenerateToken(IEnumerable<Claim>? claims);

        public Task<TokenValidationResult> VerifyAccessToken(string token);
        public Task<RefreshTokenValidationResultEntity> VerifyRefreshToken(OidcRequestDto parameter);

    }
}
