using Rubik.Identity.Oidc.Core.OidcEntities;
using Rubik.Identity.Oidc.Core.Stores;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.AuthServer.Stores
{
    public class ClientStore(IFreeSql freeSql) : IClientStore
    {
        public async Task<ClientEntity?> GetClient(string client_id)
        {
            var app = await freeSql.Select<TbApplication>()
                .Where(a => a.IsDelete == false && a.Code == client_id)
                .FirstAsync();

            if(app==null)
                return null;

            return new ClientEntity
            {
                ClientID= app.Code,
                CallbackPath = app.CallbackPath,
                ClientSecret = app.ClientSecret,
                ResponseType=app.ResponseType.ToString()!.Replace(",",""),
                Scope = app.Scope,
            };
        }
    }
}
