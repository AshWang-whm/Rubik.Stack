using Rubik.Identity.OidcReferenceAuthentication;

namespace JwtTest
{
    public static class JwtAuthencationExtension
    {
        /// <summary>
        /// 自定义验证中间件，直接远程访问oidc server token接口进行验证
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        public static void AddOidcReferenceAuthencation(this WebApplicationBuilder builder,string? scheme = null, Action<OidcReferenceAuthenticationOptions>? options = null)
        {
            scheme ??= OidcReferenceDefaults.AuthenticationScheme;

            builder.Services.AddHttpContextAccessor();

            builder.Services
                .AddAuthorization()
                .AddAuthentication(scheme)
                .AddOidcReferenceAuthencation(o =>
                {
                    options?.Invoke(o);
                    o.Authority = "http://localhost:5000";
                    o.Events = new OidcReferenceEvents
                    {
                        OnMessageReceived = context =>
                        {
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public static void AddOidcReferenceAuthencation(this WebApplicationBuilder builder,  Action<OidcReferenceAuthenticationOptions>? options = null)
        {
            builder.AddOidcReferenceAuthencation(OidcReferenceDefaults.AuthenticationScheme, options);
        }

    }
}
