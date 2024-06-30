using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Extension
{
    public static class HttpcontextAccessorExtension
    {
        public static bool CheckRole(this IHttpContextAccessor httpContextAccessor,string roles,string claim="role")
        {
            var val = httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(a => a.ValueType == claim)?.Value;
            if(val == null) 
                return false;
            else
                return roles.Split(',').Any(roles => val == roles);
        }
    }
}
