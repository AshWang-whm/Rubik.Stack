using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public class OidcReferenceEvents
    {
        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

        public Func<TokenValidatedContext, Task>? OnTokenValidated { get; set; } = DefaultValidated;

        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);

        /// <summary>
        /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
        /// </summary>
        public virtual Task TokenValidated(TokenValidatedContext context) => OnTokenValidated == null ? DefaultValidated(context) : OnTokenValidated(context);

        static Task DefaultValidated(TokenValidatedContext context)
        {
            if (context.Options.VerifyAudience)
            {
                var subs = context.Principal.Claims.Where(a=>a.Type== JwtRegisteredClaimNames.Aud);
                if (!subs?.Any(a=>a.Value==context.Options.Audience)??true)
                {
                    context.Fail("Invalid Audience!");
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
