using Rubik.Identity.Oidc.Core.Attributes;
using Rubik.Identity.Oidc.Core.OidcEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Stores
{
    [AutoInject( AutoInjectType.Scope)]
    public interface IApiResourceStore
    {
        public Task<List<ApiResourceEntity>> GetApiResources(string scope);
    }
}
