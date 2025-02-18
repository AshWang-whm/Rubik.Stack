using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public static class PolicyExtensions
    {
        public static bool CheckClaimValue(this AuthorizationHandlerContext context,string claim,string value)
        {
            var scope = context.User.Claims.FirstOrDefault(a => a.Type == claim);
            if (scope == null)
                return false;
            return scope.Value.Contains(value);
        }

        public static bool CheckClaimValues(this AuthorizationHandlerContext context, string claim, IEnumerable<string> values)
        {
            var scope = context.User.Claims.FirstOrDefault(a => a.Type == claim);
            if (scope == null)
                return false;
            return !values.Except(scope.Value.Split(' ')).Any();
        }
    }
}
