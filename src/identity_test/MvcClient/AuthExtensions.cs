using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MvcClient
{
    public static class AuthExtensions
    {
        public static void AddOpenIDAuth(this IServiceCollection services)
        {
            // mvc端直接用cookies做系统认证
            // 前后端分离系统，前端自行处理refresh token 1：定时刷新token 2：401时再刷新token
            services.AddAuthentication("oidc")
            .AddCookie("cookie", o =>
            {
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddOpenIdConnect("oidc", o =>
            {
                o.SignInScheme = "cookie";
                o.RequireHttpsMetadata = false;
                o.ClientId = "mvc_client";
                // 仅发送到idp验证用
                o.ClientSecret = "ClientSecretClientSecretClientSecretClientSecret";

                o.UsePkce = true;
                o.SaveTokens = true;

                o.CallbackPath = "/oidc/callback";
                o.Authority = "http://localhost:5000";
                o.ClaimsIssuer = "http://localhost:5000";

                // Response Type 包含 code，client端需要生成code_challenge并发送到server，server端加密code_challenge&其他数据，生成code_verifier然后返回给client端，
                // client端请求Token流程会携带code_challenge和code_verifier参数，server端需要验证两个参数
                // code ResponseType有code_challenge，code_challenge
                o.ResponseType = OpenIdConnectResponseType.Code;
                //o.GetClaimsFromUserInfoEndpoint = true;

                // 弱智openid 文档，Query 模式毫无头绪
                //o.ResponseMode = OpenIdConnectResponseMode.Query;

                // 设置需要验证的 openid connect 协议数据
                // RequireNonce=false ， 就不会生成nonce发送到server端 , 但是state 为null的异常无法解决，不管是url，form，id token中都无法处理该异常
                //o.ProtocolValidator = new OpenIdConnectProtocolValidator { RequireNonce = false, RequireSub = true,RequireState=true,RequireStateValidation=true,IdTokenValidator=(idtoken,context)=> { }};

                o.Scope.Add("openid");
                o.Scope.Add("profile");
                o.Scope.Add("scope1");
                //o.Scope.Add("offline_access");

                o.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenResponseReceived = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                };
            });
        }

    }
}
