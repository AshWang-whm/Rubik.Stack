using Rubik.Identity.Admin.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Rubik.Identity.FreesqlExtension;
using Rubik.Infrastructure.WebExtension;
using Rubik.Identity.OidcReferenceAuthentication;
using Rubik.Identity.Admin.Apis;

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
    // ���� aop
});

//builder.Services.AddHttpClient();

builder.Services.AddAntDesign();

// ��ǰϵͳ��¼��֤
builder.Services.AddAuthentication("oidc")
            .AddCookie("cookie", o =>
            {
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddOpenIdConnect("oidc", o =>
            {
                o.SignInScheme = "cookie";
                o.RequireHttpsMetadata = false;
                o.ClientId = "app_admin";
                // �����͵�idp��֤��
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
                        // ��ȡ ID Token
                        if (context.SecurityToken is JwtSecurityToken idToken)
                        {
                            // ���� ID Token �е��û���Ϣ

                            // �����µ� ClaimsIdentity
                            var identity = new ClaimsIdentity(idToken.Claims, context.Principal!.Identity!.AuthenticationType);

                            // �����µ� ClaimsPrincipal
                            var principal = new ClaimsPrincipal(identity);

                            // �滻 HttpContext.User
                            context.Principal = principal;
                            context.Success();
                        }

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
        // �����֤scope ����app_admin ?
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

// �ⲿȨ�޽ӿ�ʹ��jwt token��֤
app.MapGet("/api/admin/permissions/{sys}", AdminApis.UserPermissions).RequireAuthorization("api");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization("admin");

app.Run();
