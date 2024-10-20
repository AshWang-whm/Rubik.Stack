using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    /// <summary>
    /// Initializes a new instance of <see cref="MessageReceivedContext"/>.
    /// </summary>
    /// <inheritdoc />
    public class MessageReceivedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        OidcReferenceAuthenticationOptions options) : ResultContext<OidcReferenceAuthenticationOptions>(context, scheme, options)
    {

        /// <summary>
        /// Bearer Token. This will give the application an opportunity to retrieve a token from an alternative location.
        /// </summary>
        public string? Token { get; set; }
    }
}
