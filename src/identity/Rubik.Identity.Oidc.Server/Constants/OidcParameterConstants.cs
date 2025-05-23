﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Constants
{
    internal class OidcParameterConstants
    {
        public const string ClientID = "client_id";
        public const string GrantType = "grant_type";
        public const string ClientSecret = "client_secret";
        public const string AccessToken = "access_token";

        public const string Implicit = "implicit";

        public const string Token = "token";

        public const string IdToken = "id_token";

        public const string RefreshToken = "refresh_token";

        public const string OfflineAccess = "offline_access";

        public const string Authorization_Code = "authorization_code";

        public const string AuthorizationFlow_Code = "code";

        public const string AuthorizationFlow_Verifier = "code_verifier";

        public const string Scope = "scope";

        public const string OpenId = "openid";

        public const string Profile = "profile";

        public const string Bearer = "Bearer";

        public const string ClientCredentialsFlow="client_credentials";

        public const string PasswordFlow = "password";

        public const string AuthorizationHeader = "Authorization";

        public const string Basic = "Basic";
    }
}
