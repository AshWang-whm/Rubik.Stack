using Microsoft.AspNetCore.Mvc;
using Rubik.Identity.UserIdentity;

namespace Rubik.Identity.Admin.Apis
{
    public class AdminApis
    {
        /// <summary>
        /// 获取用户sys应用的菜单/权限
        /// </summary>
        /// <param name="freeSql"></param>
        /// <param name="identity"></param>
        /// <param name="sys"></param>
        /// <returns></returns>
        public static async Task<string> UserPermissions([FromServices]IFreeSql freeSql, [FromServices]UserIdentityAccessor identity,string sys)
        {
            // 获取用户的sys下的权限


            return "ok";
        }
    }
}
