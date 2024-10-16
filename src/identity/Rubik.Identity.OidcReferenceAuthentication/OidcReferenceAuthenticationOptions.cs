using Microsoft.AspNetCore.Authentication;
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

        internal string VerifyEndpointFormat => $"{VerifyEndpoint}{(VerifyEndpoint.EndsWith('/')?"":"/")}{{0}}";

        public string? Authority { get; set; }

        /// <summary>
        /// 是否保存解析token并保存claims
        /// </summary>
        public bool SaveClaims { get; set; }= true;
    }
}
