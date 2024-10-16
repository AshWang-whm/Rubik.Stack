using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public static class OidcReferenceExtensions
    {
        public static AuthenticationBuilder AddOidcReferenceAuthencation(this AuthenticationBuilder builder)
            => builder.AddOidcReferenceAuthencation(OidcReferenceDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddOidcReferenceAuthencation(this AuthenticationBuilder builder, string authenticationScheme)
       => builder.AddOidcReferenceAuthencation(authenticationScheme, _ => { });

        public static AuthenticationBuilder AddOidcReferenceAuthencation(this AuthenticationBuilder builder, Action<OidcReferenceAuthenticationOptions> configureOptions)
        => builder.AddOidcReferenceAuthencation(OidcReferenceDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddOidcReferenceAuthencation(this AuthenticationBuilder builder, string authenticationScheme, Action<OidcReferenceAuthenticationOptions> configureOptions)
        => builder.AddOidcReferenceAuthencation(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddOidcReferenceAuthencation(this AuthenticationBuilder builder, string authenticationScheme, string? displayName
            , Action<OidcReferenceAuthenticationOptions> configureOptions)
        {
            var authencationOptions = new OidcReferenceAuthenticationOptions();
            configureOptions?.Invoke(authencationOptions);
            builder.Services.AddHttpClient(OidcReferenceDefaults.ReferenceHttpClient, opts =>
            {
                opts.BaseAddress = new Uri(authencationOptions.Authority);
            });

            return builder.AddScheme<OidcReferenceAuthenticationOptions, OidcReferenceAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
