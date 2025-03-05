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
        public const string Sub = JwtRegisteredClaimNames.Sub;

        /// <summary>
        /// name
        /// </summary>
        public const string Name = JwtRegisteredClaimNames.Name;

        public const string Dept = "dept";

        /// <summary>
        /// role
        /// </summary>
        public const string Role = "role";

        public const string Job = "job";

        public const string Position = "pos";
    }
}
