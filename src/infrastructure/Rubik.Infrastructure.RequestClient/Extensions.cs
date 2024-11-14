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

        public static void AddRequestClient<THandler>(this IServiceCollection services, Uri uri, string client = RequestClient.ClientName)
            where THandler:class, IRequestClientHandler
        {
            services.AddHttpClient(client, opts =>
            {
                opts.BaseAddress = uri;
            });

            services.AddScoped<RequestClient>();
            services.AddKeyedScoped<THandler>($"{RequestClient.HandlerPerfix}_{client}");

        }

        public static void AddRequestClient(this IServiceCollection services, string uri,  string client = RequestClient.ClientName)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var clientUri))
                throw new Exception($"[{uri}]格式错误!");

            services.AddRequestClient(clientUri, client);
        }

        public static void AddRequestClient<THandler>(this IServiceCollection services, string uri, string client = RequestClient.ClientName)
            where THandler : class, IRequestClientHandler
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var clientUri))
                throw new Exception($"[{uri}]格式错误!");

            services.AddRequestClient<THandler>(clientUri, client);
        }
    }
}
