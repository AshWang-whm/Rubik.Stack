using Rubik.Identity.Admin.Components;
using Rubik.Infrastructure.OAuth;
using Rubik.Share.Entity.FreesqlExtension;
using Rubik.Share.Extension;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddEnvironmentJsonFile();

builder.Services.AddHttpContextAccessor();

var fsql = builder.AddFreesql("identity", FreeSql.DataType.PostgreSQL, cmd =>
{
#if DEBUG
    System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
    // ���� aop
});

//builder.Services.AddHttpClient();

builder.Services.AddAntDesign();

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
            });

builder.Services.AddUserIdentity();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();

app.Run();
