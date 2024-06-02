using Rubik.Identity.Share.Entity.AuthEntities;

namespace Rubik.Identity.AuthServer.Stores
{
    public interface IClientStore
    {
        Task<List<AuthClientEntity>> GetClients();

        Task<AuthClientEntity> GetClient(string clientId);
    }
}
