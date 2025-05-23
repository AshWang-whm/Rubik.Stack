﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rubik.Identity.Oidc.Core.Services;
using Rubik.Identity.Oidc.Core.Stores;

namespace Rubik.Identity.Oidc.Core.Endpoints
{
    internal class UserInfoEndpoint
    {
        public static async Task<IResult> GetUserInfo([FromServices] HttpContextService httpContextService
            ,[FromServices]IUserStore userStore)
        {
            var userid = httpContextService.GetSID();



            return Results.Json(new { });
        }

        public static async Task<IResult> Logout([FromServices]IHttpContextAccessor httpContextAccessor
            , [FromServices] IAuthenticationSchemeProvider schemeProvider
            , [FromServices] IAuthenticationHandlerProvider authenticationHandler)
        {
            var default_scheme = await schemeProvider.GetDefaultAuthenticateSchemeAsync();
            if(default_scheme != null)
            {
                await httpContextAccessor.HttpContext!.SignOutAsync(default_scheme!.Name);

                var redirect_url = httpContextAccessor.HttpContext?.Request.Query["post_logout_redirect_uri"];
                if (redirect_url.HasValue && Uri.TryCreate(redirect_url, UriKind.Absolute, out var url))
                    return Results.Redirect(url.AbsoluteUri);
                else
                {
                    var default_handler = await authenticationHandler.GetHandlerAsync(httpContextAccessor.HttpContext!, default_scheme!.Name);
                    if (default_handler is CookieAuthenticationHandler cookieHandler)
                    {
                        return Results.Redirect(cookieHandler.Options.LoginPath);
                    }
                }
            }

            return Results.Redirect("/"); 
        }
    }
}
