using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.OidcEntities
{
    public class ApiResourceEntity
    {
        public string? Name { get; set; }
        public string? Code { get; set; }

        public List<ApiScopeEntity> Scopes { get; set; } = [];

    }
}
