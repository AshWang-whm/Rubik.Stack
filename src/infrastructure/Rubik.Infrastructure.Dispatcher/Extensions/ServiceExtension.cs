using Microsoft.Extensions.DependencyInjection;
using Rubik.Infrastructure.Dispatcher.Default;
using Rubik.Infrastructure.Dispatcher.Internal;

namespace Rubik.Infrastructure.Dispatcher.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services,ServiceLifetime lifetime= ServiceLifetime.Singleton)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Distinct().SelectMany(a => a.GetTypes()).ToArray();
            // 注入调度器
            services.AddSingleton(new Internal.Dispatcher(services, types).Build(lifetime));

            // 注入middleware
            var middlewares = types.Where(a => a.IsAssignableTo(typeof(IEventMiddleware)) && a.IsConcrete());
            foreach (var item in middlewares)
            {
                services.Add(new ServiceDescriptor(typeof(IEventMiddleware), item, lifetime));
                //services.AddScoped(typeof(IEventMiddleware), item);
            }

            //services.AddScoped(typeof(IEventBus), typeof(EventBus));
            services.Add(new ServiceDescriptor(typeof(IEventBus), typeof(EventBus), lifetime));
            return services;
        }

        /// <summary>
        /// 默认的Provider,Filter EventBus
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus<THandler>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where THandler: EventHandlerAttribute, new()
        {
            return services.AddEventBus<THandler, DefaultEventHandlerProvider<THandler>>(lifetime);
        }

        public static IServiceCollection AddEventBus<THandler,TEventProvider>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where THandler : EventHandlerAttribute
            where TEventProvider : EventHandlerProvider<THandler>,new()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Distinct().SelectMany(a => a.GetTypes()).ToArray();

            var provider = new TEventProvider();
            services.AddSingleton(provider);

            var eventTypes = provider.GetAddServiceTypeList();
            foreach (var type in eventTypes)
            {
                services.Add(new ServiceDescriptor(type,type,lifetime));
                //services.AddScoped(type);
            }

            services.Add(new ServiceDescriptor(typeof(Dispatcher<THandler, TEventProvider>),typeof(Dispatcher<THandler, TEventProvider>),lifetime));
            //services.AddScoped<Dispatcher<THandler, TEventProvider>>();

            ////// 注入middleware
            var middlewares = types.Where(a =>a.IsConcrete()&& a.GetInterfaces().Any(s => s.IsGenericType && s.GetGenericTypeDefinition() == typeof(IGenericsEventMiddleware<,>)));
            foreach (var item in middlewares)
            {
                var middleware_interfaces = item.GetInterfaces().First();
                //services.AddScoped(middleware_interfaces, item);
                services.Add(new ServiceDescriptor(middleware_interfaces, item, lifetime));
            }

            ////// 注入filter 
            var filters = types.Where(a =>a.IsConcrete()&& a.GetInterfaces().Any(s => s.IsGenericType && s.GetGenericTypeDefinition() == typeof(IEventHandlerFilter<,>)));
            foreach (var item in filters)
            {
                var filter = item.GetInterfaces().First();
                //services.AddScoped(filter, item);
                services.Add(new ServiceDescriptor(filter, item, lifetime));
            }

            //services.AddScoped(typeof(IEventBus<THandler>), typeof(EventBus<THandler, TEventProvider>));
            services.Add(new ServiceDescriptor(typeof(IEventBus<THandler>), typeof(EventBus<THandler, TEventProvider>), lifetime));
            return services;
        }
    }
}
