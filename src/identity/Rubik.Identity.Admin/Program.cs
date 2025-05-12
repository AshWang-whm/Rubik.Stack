using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Rubik.Identity.Admin.Apis;
using Rubik.Identity.Admin.Components;
using Rubik.Identity.FreesqlExtension;
using Rubik.Identity.OidcReferenceAuthentication;
using Rubik.Infrastructure.WebExtension;

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

                // fragment: 允许access/id token 在url query中返回，用于token,id token 的模式
                // query ： 与fragment 相反， 用于code 模式
                //o.ResponseMode = "fragment";
                // 非code模式应该弃用pkce , false 时code模式不会发送 code_verifier 到token endpoint
                o.UsePkce = true;
                o.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;
                //o.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;


                o.CallbackPath = "/oidc/callback";
                o.Authority = "http://localhost:5000";
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = "rubik.oidc",
                    //ValidAudience="app_admin", // 单个aud
                    ValidAudiences = ["app_admin"], // id token有多个aud情况下，验证其中1个
                    ValidateAudience=true
                };

                o.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                {
                    OnTokenResponseReceived = async context =>
                    {
                        await Task.CompletedTask;
                    },
                    OnMessageReceived = async context =>
                    {

                        
                    },
                    OnRedirectToIdentityProvider = async context =>
                    {
                    }
                };


                o.Scope.Add("openid");
                o.Scope.Add("profile");
                o.Scope.Add("offline_access");
                // openid profile offline_access 
                o.Scope.Add("api.test.scope1");
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
