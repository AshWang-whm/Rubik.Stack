using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rubik.Identity.OidcReferenceAuthentication
{
    public class OidcReferenceAuthenticationOptions: AuthenticationSchemeOptions
    {
        public string Scheme { get; set; } = "Reference";

        /// <summary>
        /// token 验证路径
        /// </summary>
        public string VerifyEndpoint { get; set; } = "/oauth/verify";

        internal string VerifyEndpointRestfulFormat => $"{VerifyEndpoint}{(VerifyEndpoint.EndsWith('/') ? "" : "/")}{{0}}";

        public string? Authority { get; set; }

        public string? Audience { get; set; }

        /// <summary>
        /// default is true
        /// </summary>
        public bool VerifyAudience { get; set; } = true;

        /// <summary>
        /// 是否保存解析token并保存claims
        /// </summary>
        public bool SaveClaims { get; set; }= true;

        public new OidcReferenceEvents Events { get=>(OidcReferenceEvents)base.Events;set { base.Events = value; } }

        public TokenValidationParameters TokenValidationParameters { get; set; } = new TokenValidationParameters();

    }
}
