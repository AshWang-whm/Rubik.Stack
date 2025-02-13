using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.OAuth
{
    public class Identity(IHttpContextAccessor httpContextAccessor)
    {
        public string? UserCode => httpContextAccessor.CurrentUserCode();
        public string? UserName => httpContextAccessor.CurrentUserName();

        public string? Roles => httpContextAccessor.CurrentUserRole();

        public string? Department => httpContextAccessor.CurrentUserDept();

        public bool CheckClaims(string type, string value) => httpContextAccessor.CheckClaims(type, value.Split(','));
    }
}
