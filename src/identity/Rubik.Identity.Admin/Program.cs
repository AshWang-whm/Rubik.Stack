using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Rubik.Identity.Admin.Apis;
using Rubik.Identity.Admin.Components;
using Rubik.Identity.FreesqlExtension;
using Rubik.Identity.OidcReferenceAuthentication;
using Rubik.Infrastructure.WebExtension;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddEnvironmentJsonFile();

var fsql = builder.AddFreesqlWithIdentityAop("identity", FreeSql.DataType.PostgreSQL, cmd =>
{
#if DEBUG
    System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
    // 补充 aop
});

//builder.Services.AddHttpClient();

builder.Services.AddAntDesign();

// 当前系统登录认证
builder.Services.AddAuthentication("oidc")
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddOpenIdConnect("oidc", o =>
            {
                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.RequireHttpsMetadata = false;
                o.ClientId = "app_admin";
                // 仅发送到idp验证用
                o.ClientSecret = "app_admin_app_admin_app_admin";

                o.SaveTokens = true;

                o.CallbackPath = "/oidc/callback";
                o.Authority = "http://localhost:5000";
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = "rubik.oidc"
                };

                o.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                {
                    OnTokenValidated = async context =>
                    {
                        await Task.CompletedTask;
                    }
                };

                o.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;

                o.Scope.Add("openid");
                o.Scope.Add("profile");
                o.Scope.Add("offline_access");
            })
            .AddOidcReferenceAuthencation(OidcReferenceDefaults.AuthenticationScheme, opt =>
            {
                opt.Authority = "http://localhost:5000";
                opt.Events = new OidcReferenceEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["access_token"];
                        return Task.CompletedTask;
                    }
                };
            });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin", policy =>
    {
        policy.AddAuthenticationSchemes("oidc")
            .RequireAuthenticatedUser();
    })
    .AddPolicy("api", policy =>
    {
        // 添加验证scope 包含app_admin ?
        policy.AddAuthenticationSchemes(OidcReferenceDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .RequireAssertion(context =>
        {
            return context.CheckClaimValue("scope", "app_admin");
        });
    });

var app = builder.Build();

await Rubik.Identity.Share.Entity.FreesqlExtension.DbInitialize(fsql);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// 外部权限接口使用jwt token验证
app.MapGet("/api/admin/permissions/{sys}", AdminApis.UserPermissions).RequireAuthorization("api");
app.MapGet("/logout", async context =>
{
    var cookieKeys = context.Request.Cookies.Keys;
    foreach (var key in cookieKeys)
    {
        context.Response.Cookies.Delete(key);
    }
    context.Response.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization("admin");

app.Run();
