using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public class TokenValidatedContext(HttpContext context, AuthenticationScheme scheme, OidcReferenceAuthenticationOptions options) 
        : ResultContext<OidcReferenceAuthenticationOptions>(context, scheme, options)
    {
    }
}
