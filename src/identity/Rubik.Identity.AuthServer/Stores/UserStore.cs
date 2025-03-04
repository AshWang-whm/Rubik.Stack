using Rubik.Identity.Oidc.Core.Stores;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;
using Rubik.Infrastructure.Utils.Common.String;
using System.Security.Claims;

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

        public async Task<List<Claim>> MapperUserClaims(string usercode, string ClientID, string scopes)
        {
            // 抓取内置的scope & 用户数据
            var claims = new List<Claim>();
            if(scopes.Contains("role"))
            {
                var roles = await freeSql.Select<TbUser, TbRelationRoleUser, TbRole,TbRelationRolePermission,TbApplicationPermission,TbApplication>()
                    .LeftJoin((a, b, c,d,e,f) => a.ID == b.UserID)
                    .LeftJoin((a, b, c, d, e, f) => b.RoleID==c.ID)
                    .LeftJoin((a, b, c, d, e, f) => c.ID==d.RoleID)
                    .LeftJoin((a, b, c, d, e, f) => d.PermissionID==e.ID)
                    .LeftJoin((a, b, c, d, e, f) => e.ApplicationID==f.ID)
                    .Where((a, b, c, d, e, f) => f.Code==ClientID&&a.Code==usercode)
                    .ToListAsync((a,b,c, d, e, f) => c.Code);
                claims.Add(new Claim("role",roles?.StringJoin()??""));
            }
            if(scopes.Contains("job"))
            {
                var jobs = await freeSql.Select<TbUser, TbRelationJobUser, TbOrganizationJob>()
                    .LeftJoin((a, b, c) => a.ID == b.UserID)
                    .LeftJoin((a, b, c) => b.JobID==c.ID)
                    .Where((a,b,c)=>a.Code==usercode)
                    .ToListAsync((a,b,c)=>c.Code);
                claims.Add(new Claim("job", jobs?.StringJoin() ?? ""));
            }
            if (scopes.Contains("pos"))
            {
                var pos = await freeSql.Select<TbUser,TbRelationPositionUser,TbPosition>()
                    .LeftJoin((a, b, c) => a.ID == b.UserID)
                    .LeftJoin((a, b, c) => b.PositionID == c.ID)
                    .Where((a,b,c)=>a.Code==usercode)
                    .ToListAsync((a, b, c) => c.Code);
                claims.Add(new Claim("pos", pos?.StringJoin() ?? ""));
            }
            return claims;
        }

        public async Task<IEnumerable<string>> UserProfilesClaims()
        {
            return ["role","job","pos","dept"];
        }
    }
}
