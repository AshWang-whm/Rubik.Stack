using Microsoft.Extensions.DependencyInjection;

namespace Rubik.Infrastructure.RequestClient
{
    public static class Extensions
    {
        public static void AddRequestClient(this IServiceCollection services,Uri uri,string client= RequestClient.ClientName)
        {
            services.AddHttpClient(client, opts =>
            {
                opts.BaseAddress = uri;
            });

            services.AddScoped<RequestClient>();
        }
    }
}
