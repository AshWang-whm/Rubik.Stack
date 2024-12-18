﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Contants
{
    internal class OidcExceptionConstant
    {
        public const string RefreshToken_Invalid = "refresh token is not valided!";

        public const string RefreshToken_IsRequired = "refresh_token is required!";

        public const string AccessToken_IsRequired = "access_token is required!";

        public const string AuthorizationCode_Invalid = "code";

        public const string GrantType_IsRequired = "grant type is invalid!";

        public const string ClientId_Invalid = "client_id is invalid!";

        public const string ResponseType_Invalid = "response_type is invalid!";

        public const string Scope_Invalid = "scope is invalid!";

        public const string UserName_IsRequired = "user name is required!";

        public const string Password_IsRequired = "password id required!";

        public const string Password_Invalid = "password is invalid!";
    }
}
