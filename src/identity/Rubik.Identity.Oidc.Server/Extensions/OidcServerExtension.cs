using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rubik.Identity.AuthServer.Endpoints;
using Rubik.Identity.Oidc.Core.Attributes;
using Rubik.Identity.Oidc.Core.Configs;
using Rubik.Identity.Oidc.Core.Endpoints;
using Rubik.Identity.Oidc.Core.RsaKey;
using Rubik.Identity.Oidc.Core.Services;
using System.Reflection;

namespace Rubik.Identity.Oidc.Core.Extensions
{
    public static class OidcServerExtension
    {
        public static WebApplicationBuilder AddOidcServer(this WebApplicationBuilder builder)
        {
            // Oidc 服务器端自己用cookie验证
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication("oidc.cookie")
                .AddCookie("oidc.cookie", o =>
                {
                    o.LoginPath = "/Account/Login";
                });

            // code 生成器
            builder.Services.AddDataProtection(opt=>opt.ApplicationDiscriminator="Rubik.Identity.AuthServer");

            // oidc services
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<HttpContextService>();
            builder.Services.AddScoped<AuthorizationCodeEncrtptService>();
            builder.Services.AddScoped<GrantTypeHandleService>();

            // stores
            builder.AutoInject();

            return builder;
        }

        public static WebApplicationBuilder AddOidcConfig(this WebApplicationBuilder builder, Action<DiscoveryConfig>? discovery = null,Action<RsaKeyConfig>? rsa=null)
        {
            var discovery_config = new DiscoveryConfig();
            discovery?.Invoke(discovery_config);

            var rsa_config = new RsaKeyConfig();
            rsa?.Invoke(rsa_config);

            var rsa_file = Path.Combine(builder.Environment.ContentRootPath, OidcServer.RsaKeyConfig.RsaKeyFilePath);
            if (!File.Exists(rsa_file))
            {
                throw new FileNotFoundException($"[{rsa_file}]文件不存在!");
            }
            rsa_config.RsaKeyFileFullPath = rsa_file;
            OidcServer.DiscoveryConfig = discovery_config;

            builder.Services.AddSingleton(rsa_config);
            builder.Services.AddSingleton<JwkRsaKeys>();
            builder.Services.AddSingleton<DiscoveryConfig>();

            return builder;
        }

        public static void UseOidcServer(this WebApplication web)
        {
            OidcServer.WebApplication = web;
            web.MapGet(OidcServer.DiscoveryConfig!.DiscoveryEndpoint, DiscoveryEndpoint.GetDiscoveryDoc);
            web.MapGet(OidcServer.DiscoveryConfig!.JwksEndpoint, JwkEndpoint.GetJwks);
            web.MapGet(OidcServer.DiscoveryConfig!.UserInfoEndpoint, UserInfoEndpoint.GetUserInfo).RequireAuthorization();
            web.MapGet(OidcServer.DiscoveryConfig!.AuthorizationEndpoint, AuthorizeEndpoint.Authorize).RequireAuthorization();
            web.MapPost(OidcServer.DiscoveryConfig!.TokenEndpoint, TokenEndoint.GetToken);
            web.MapGet(OidcServer.DiscoveryConfig!.VerifyTokenEndpoint, TokenEndoint.VerifyReferenceToken);
            web.MapGet(OidcServer.DiscoveryConfig!.VerifyTokenRestEndpoint, TokenEndoint.VerifyReferenceToken);
        }

        static void AutoInject(this WebApplicationBuilder builder)
        {
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            // 加载所需
            var types = ass.SelectMany(a => a.GetTypes()).ToArray();

            var auto_inject = types
                .Select(a=>new { Type=a,Attribute=a.GetCustomAttribute<AutoInjectAttribute>()})
                .Where(a => a.Attribute != null);
            foreach (var item in auto_inject)
            {
                // 
                var instances = types.Where(a=>a.IsClass&&a.IsAssignableTo(item.Type)).ToArray();
                builder.AddService(item.Attribute!,item.Type, instances);
            }
        }

        static void AddService(this WebApplicationBuilder builder,AutoInjectAttribute autoInject,Type serviceType, params Type[] impls)
        {
            foreach (var item in impls)
            {
                switch (autoInject.InjectType)
                {
                    case AutoInjectType.Transient:
                        builder.Services.AddTransient(serviceType, item);
                        break;
                    case AutoInjectType.Scope:
                        builder.Services.AddScoped(serviceType, item);
                        break;
                    case AutoInjectType.Singleton:
                        builder.Services.AddSingleton(serviceType, item);
                        break;
                }
            }
            
        }
    }
}
