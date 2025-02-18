using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.UserIdentity
{
    public class JwtIdentityClaimConstants
    {

        /// <summary>
        /// sub
        /// </summary>
        public const string Code = JwtRegisteredClaimNames.Sub;

        /// <summary>
        /// name
        /// </summary>
        public const string Name = JwtRegisteredClaimNames.Name;

        public const string Dept = "dept";

        /// <summary>
        /// role
        /// </summary>
        public const string Role = "role";
    }
}
