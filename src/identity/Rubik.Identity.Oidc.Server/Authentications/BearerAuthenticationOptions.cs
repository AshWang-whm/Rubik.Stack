using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Authentications
{
    internal class BearerAuthenticationOptions: AuthenticationSchemeOptions
    {
        public static string BearerScheme = "Rubik.BearerScheme";
        public static string BearerPolicy = "Rubik.BearerPolicy";
    }
}
