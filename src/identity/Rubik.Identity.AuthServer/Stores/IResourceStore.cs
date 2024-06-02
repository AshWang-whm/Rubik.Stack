using Rubik.Identity.Share.Entity.AuthEntities;
namespace Rubik.Identity.AuthServer.Stores
{
    public interface IResourceStore
    {
        Task<List<AuthApiResourceEntity>> GetApiResources();
        Task<AuthApiResourceEntity> GetApiResource(string api);

    }
}
