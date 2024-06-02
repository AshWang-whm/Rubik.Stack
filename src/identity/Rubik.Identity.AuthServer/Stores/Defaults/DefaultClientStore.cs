using Rubik.Identity.Share.Entity.AuthEntities;

namespace Rubik.Identity.AuthServer.Stores.Defaults
{
    public class DefaultClientStore : IClientStore
    {
        public Task<AuthClientEntity> GetClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuthClientEntity>> GetClients()
        {
            throw new NotImplementedException();
        }
    }
}
