using Rubik.Identity.Oidc.Core.Constants;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Dtos
{
    public class RequestOidcParameterDto
    {
        public string? GrantType { get; set; }

        public required string ClientID { get; set; }

        public string? ClientSecret { get; set; }

        public string? Response_Type { get; set; }

        public required NameValueCollection Query { get; set; }

        public bool IsAccessToken => Response_Type?.Split(' ').Contains(OidcParameterConstants.Token) ?? false;

        public bool IsIdToken => Response_Type?.Split(' ').Contains(OidcParameterConstants.IdToken) ?? false;

    }
}
