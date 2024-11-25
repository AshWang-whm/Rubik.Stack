using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Contants
{
    internal class OidcParameterContanst
    {
        public const string AccessToken = "access_token";

        public const string IdToken = "id_token";

        public const string RefreshToken = "refresh_token";

        public const string OfflineAccess = "offline_access";

        public const string Authorization_Code = "authorization_code";

        public const string AuthorizationFlow_Code = "code";

        public const string AuthorizationFlow_Verifier = "code_verifier";

        public const string Scope = "scope";

        public const string Bearer = "Bearer";

        public const string ClientCredentialsFlow="client_credentials";

        public const string PasswordFlow = "password";
    }
}
