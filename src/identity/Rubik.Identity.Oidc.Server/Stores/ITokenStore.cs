using Microsoft.IdentityModel.Tokens;
using Rubik.Identity.Oidc.Core.Attributes;
using Rubik.Identity.Oidc.Core.Dtos;
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
    
    [AutoInject(AutoInjectType.Scope)]
    public interface ITokenStore
    {
        public string GenerateToken(OidcQueryParameterDto parameter, IEnumerable<Claim>? claims);

        public Task<TokenValidationResult> VerifyAccessToken(string token);
        public Task<RefreshTokenValidationResultEntity> VerifyRefreshToken(OidcQueryParameterDto parameter);

    }
}
