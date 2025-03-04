using BlazorServerTest.Components;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
                o.ClientId = "blazor_server_test";
                // 仅发送到idp验证用
                o.ClientSecret = "ClientSecretClientSecretClientSecretClientSecret";

                o.SaveTokens = true;

                o.CallbackPath = "/oidc/callback";
                o.Authority = "http://localhost:5000";
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = "rubik.oidc"
                };

                o.ResponseType = OpenIdConnectResponseType.Code;

                o.Scope.Add("openid");
                o.Scope.Add("profile");
                o.Scope.Add("scope1");
                o.Scope.Add("offline_access");
                o.Scope.Add("role");
                o.Scope.Add("job");
                o.Scope.Add("pos");
                o.Scope.Add("dept");
            });

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

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
