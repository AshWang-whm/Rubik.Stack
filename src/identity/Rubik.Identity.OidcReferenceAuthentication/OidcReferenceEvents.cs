using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public class OidcReferenceEvents
    {
        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

        public Func<TokenValidatedContext, Task>? OnTokenValidated { get; set; }

        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);

        /// <summary>
        /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
        /// </summary>
        public virtual Task TokenValidated(TokenValidatedContext context) => OnTokenValidated == null ? Task.CompletedTask:OnTokenValidated(context);


    }
}
