using Rubik.Identity.Oidc.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Dtos
{
    internal class TokenJsonParameterDto
    {
        public required string ClientID { get; set; }
        public string? UserCode { get; set; }
        public string? Nonce { get; set; }
        public required string Scope { get; set; }
        public required string ResponseType { get; set; }

        public string[] ResponseTypeArray => ResponseType.Split(' ');

        public string[] ScopeArray => Scope?.Split(' ') ?? [];

        public bool IsAccessToken => ResponseTypeArray.Contains(OidcParameterConstants.Token) || ScopeArray.Where(a=>a!= OidcParameterConstants.Profile&&a!=OidcParameterConstants.OpenId).Any();

        public bool IsIdToken => ResponseTypeArray.Contains(OidcParameterConstants.IdToken) || ScopeArray.Contains(OidcParameterConstants.OpenId);
    }
}
