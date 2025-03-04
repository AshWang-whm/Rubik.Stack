using Rubik.Identity.Oidc.Core.Attributes;
using System.Security.Claims;

namespace Rubik.Identity.Oidc.Core.Stores
{
    [AutoInject(AutoInjectType.Scope)]
    public interface IUserStore
    {
        /// <summary>
        /// 返回用户信息配对的claims
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> UserProfilesClaims();

        public Task<bool> CheckUser(string username,string password);

        public Task<List<Claim>> MapperUserClaims(string usercode, string ClientID, string scopes);
    }
}
