using Microsoft.AspNetCore.Builder;
using Rubik.Identity.Oidc.Core.Configs;

namespace Rubik.Identity.Oidc.Core
{
    public class OidcServer
    {
        public static WebApplication? WebApplication { get; set; }

        public static DiscoveryConfig DiscoveryConfig { get; set; } = new();

        public static RsaKeyConfig RsaKeyConfig { get; set; } = new();
    }
}
