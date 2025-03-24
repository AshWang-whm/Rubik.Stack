using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Dtos
{
    public class OidcQueryParameterDto
    {
        public required string GrantType { get; set; }

        public required string ClientID { get; set; }

        public string? ClientSecret { get; set; }

        public required NameValueCollection Query { get; set; }

    }
}
