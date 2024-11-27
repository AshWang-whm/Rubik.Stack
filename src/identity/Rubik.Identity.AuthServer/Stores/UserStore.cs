using Rubik.Identity.Oidc.Core.Stores;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;

namespace Rubik.Identity.AuthServer.Stores
{
    public class UserStore (IFreeSql freeSql) : IUserStore
    {
        public async Task<bool> CheckUser(string username, string password)
        {
            var user = await freeSql.Select<TbUser>()
                .Where(a => a.Code == username)
                .FirstAsync();

            if (user == null)
            {
                return false;
            }

            var pwd = PasswordEncryptExtension.GeneratePasswordHash(username, password);

            return user.Password== pwd;
        }
    }
}
