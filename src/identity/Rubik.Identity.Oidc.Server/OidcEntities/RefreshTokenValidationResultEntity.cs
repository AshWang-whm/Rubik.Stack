using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.OidcEntities
{
    public class RefreshTokenValidationResultEntity
    {
        public bool IsValid { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public string? Exception { get; set; }
    }
}
