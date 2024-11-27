using Rubik.Identity.Oidc.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Stores
{
    [AutoInject(AutoInjectType.Scope)]
    public interface IUserStore
    {
        public Task<bool> CheckUser(string username,string password);
    }
}
