using Rubik.Identity.Oidc.Core.Attributes;
using Rubik.Identity.Oidc.Core.OidcEntities;

namespace Rubik.Identity.Oidc.Core.Stores
{
    [AutoInject(AutoInjectType.Scope)]
    public interface IClientStore
    {
        public Task<ClientEntity?> FindClientByID(string client_id);
    }
}
