using Microsoft.IdentityModel.Tokens;
using Rubik.Identity.Oidc.Core.OidcEntities;
using Rubik.Identity.Oidc.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Stores
{
    public interface ITokenStore
    {
        public string GeneratorAccessToken(TokenEndpointParameter parameter, List<Claim>? claims);

        public Task<TokenValidationResult> VerifyAccessToken(string token);
        public Task<RefreshTokenValidationResultEntity> VerifyRefreshToken(TokenEndpointParameter parameter);

    }
}
