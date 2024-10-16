using System;
using System.Collections.Generic;
using System.Text;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public class OidcReferenceDefaults
    {
        public static string AuthenticationScheme { get; set; } = "OidcReference";

        public static string ReferenceHttpClient { get; set; } = "OidcReferenceHttpClient";

    }
}
