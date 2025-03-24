using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Dtos
{
    internal class GrantTypeHandleDto
    {
        public required string ClientID { get; set; }
        public string? UserCode { get; set; }
        public string? Nonce { get; set; }
        public required string Scope { get; set; }
        public string[]? ScopeArr => Scope?.Split(' ');
    }
}
