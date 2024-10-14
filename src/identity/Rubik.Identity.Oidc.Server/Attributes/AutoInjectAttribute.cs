using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    internal class AutoInjectAttribute(AutoInjectType InjectType) : Attribute
    {
        public AutoInjectType InjectType { get; set; } = InjectType;
    }

    internal enum AutoInjectType
    {
        Transient,
        Scope,
        Singleton
    }
}
