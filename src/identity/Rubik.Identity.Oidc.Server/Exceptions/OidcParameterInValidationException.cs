using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Exceptions
{
    public class OidcParameterInValidationException : Exception
    {
        public OidcParameterInValidationException():base()
        {
            
        }

        public OidcParameterInValidationException(string msg):base (msg) 
        {
            
        }

        public static void NotNullOrEmpty(string parameter,string? val)
        {
            if (string.IsNullOrEmpty(val))
                throw new OidcParameterInValidationException($"{parameter} is required!");
        }
    }
}
