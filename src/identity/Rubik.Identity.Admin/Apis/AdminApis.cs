using Microsoft.AspNetCore.Mvc;
using Rubik.Identity.UserIdentity;

namespace Rubik.Identity.Admin.Apis
{
    public class AdminApis
    {
        public static async Task<string> UserPermissions([FromServices]IFreeSql freeSql, [FromServices]UserIdentityAccessor identity,string sys)
        {
            // 获取用户的sys下的权限


            return "ok";
        }
    }
}
