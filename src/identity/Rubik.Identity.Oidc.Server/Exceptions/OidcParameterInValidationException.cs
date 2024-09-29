using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Exceptions
{
    public class OidcParameterInValidationException(string msg) : Exception
    {
        /// <summary>
        /// 输出异常信息
        /// </summary>
        /// <returns></returns>
        public string ToException()
        {
            return msg;
        }

        public static void NotNullOrEmpty(string parameter,string? val)
        {
            if (string.IsNullOrEmpty(val))
                throw new OidcParameterInValidationException($"{parameter} is required!");
        }
    }
}
