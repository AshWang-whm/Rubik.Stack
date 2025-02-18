using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Rubik.Identity.OidcReferenceAuthentication;
using System.Text.Json;

namespace JwtTest
{
    public static class JwtAuthencationExtension
    {
        /// <summary>
        /// jwt bearer 默认实现，本地验证token模式，或二次远程访问oidc server token接口验证
        /// </summary>
        /// <param name="builder"></param>
        public static void AddTestJwtAuthencation(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient("OidcServer", opt =>
            {
                opt.BaseAddress = new Uri("http://localhost:5000");
            });

            builder.Services
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.Authority = "http://localhost:5000";
                    o.ClaimsIssuer = "rubik.oidc";
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            // 从Authorize ， Cookies ， Url 3个其中之一获取access token
                            ctx.Token = ctx.HttpContext.Request.Cookies["access_token"];
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async ctx =>
                        {
                            // 链接远程 VerifyReferenceToken , /oauth/verify
                            var client = ctx.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>()
                                .CreateClient("OidcServer");
                            if (client == null)
                            {
                                ctx.Fail(new Exception("未设置远程OidcServer HttpClient!"));
                                return;
                            }
                            var a = ctx.SecurityToken.UnsafeToString();
                            var b = ctx.SecurityToken.GetType();
                            var reference = await client!.GetStringAsync($"/oauth/verify?token={ctx.SecurityToken.UnsafeToString()}");
                            var result = JsonSerializer.Deserialize<VerifyToken>(reference);
                            if (result?.result ?? false)
                                ctx.Success();
                            else
                                ctx.Fail(new Exception(result?.exception ?? "验证token失败!"));
                        }
                    };
                }); 
        }

        /// <summary>
        /// 自定义验证中间件，直接远程访问oidc server token接口进行验证
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        public static void AddOidcReferenceAuthencation(this WebApplicationBuilder builder,string? scheme = null, Action<OidcReferenceAuthenticationOptions>? options = null)
        {
            scheme ??= OidcReferenceDefaults.AuthenticationScheme;

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
                            context.Token = context.Request.Cookies["access_token"];
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
