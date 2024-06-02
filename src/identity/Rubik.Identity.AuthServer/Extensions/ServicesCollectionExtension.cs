using Rubik.Identity.AuthServer.Configs;

namespace Rubik.Identity.AuthServer.Extensions
{
    public static class ServicesCollectionExtension
    {
        public static void AddDiscovery(this IServiceCollection services,Action<DiscoveryConfig>? config)
        {
            services.AddSingleton<DiscoveryConfig>();
        }
    }
}
