using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Contants
{
    internal class OidcExceptionContanst
    {
        public const string RefreshToken_Invalid = "refresh token is not valided!";

        public const string RefreshToken_NotFound = "refresh_token is not found in query!";

        public const string AccessToken_NotFound = "access_token is not found in query!";

        public const string AuthorizationCode_Invalid = "code";

        public const string GrantType_NotFound = "grant type not found!";

        public const string ClientId_Invalid = "client_id is invalid!";

        public const string ResponseType_Invalid = "response_type is invalid!";

        public const string Scope_Invalid = "scope is invalid!";
    }
}
