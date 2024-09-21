using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Rubik.Infrastructure.Dispatcher.Internal
{
    internal class Dispatcher(IServiceCollection services, Type[] types) : DispatcherBase
    {
        public Dispatcher Build(ServiceLifetime lifetime)
        {
            AddRelationShip();

            foreach (var dispatchInstance in GetAddServiceTypeList())
            {
                services.Add(new ServiceDescriptor(dispatchInstance, dispatchInstance, lifetime));
            }
            Build();
            return this;
        }

        private void AddRelationShip()
        {
            foreach (var type in types)
            {
                if (!type.IsConcrete())
                {
                    continue;
                }

                foreach (var method in type.GetMethods())
                {
                    AddRelationShip(type, method);
                }
            }
        }
        private void AddRelationShip(Type type, MethodInfo method)
        {
            try
            {
                var attribute = method.GetCustomAttributes<EventHandlerAttribute>().FirstOrDefault();
                if (attribute is EventHandlerAttribute handler&&nameof(EventHandlerAttribute)== attribute.GetType().Name)
                {
                    // 获取继承了Event类型的参数
                    var parameters = method.GetParameters().Where(para => typeof(Event).IsAssignableFrom(para.ParameterType))
                        .ToList();

                    if (parameters.Count != 1)
                        return;

                    var parameter = parameters[0];
                    handler.ActionMethodInfo = method;
                    handler.InstanceType = type;
                    handler.EventType = parameter.ParameterType;
                    handler.BuildExpression();
                    // 关联Event和当前Method
                    AddRelationShip(parameter.ParameterType, handler);
                }
            }
            catch (Exception)
            {
                throw new Exception($"Dispatcher: Failed to get EventBus network, type name: [{(type.FullName ?? type.Name)}], method: [{method.Name}]");
            }
        }
    }

    public class Dispatcher<THandler, TProvider>
        where THandler : EventHandlerAttribute
        where TProvider: EventHandlerProvider<THandler>
    {
        private readonly TProvider eventHandlerProvider;

        public Dispatcher(TProvider eventHandlerProvider)
        {
            this.eventHandlerProvider = eventHandlerProvider;
        }

        public async Task Publish<TEvent>(IServiceProvider serviceProvider,TEvent @event)
            where TEvent:IEvent<THandler>
        {
            var eventType = @event.GetType();
            if (!eventHandlerProvider.TryGetValue(eventType, out var handlers))
            {
                throw new Exception($"The {eventType.FullName} Handler method was not found.");
            }
            // handlers filter
            var filters = serviceProvider.GetRequiredService<IEnumerable<IEventHandlerFilter<TEvent, THandler>>>();
            foreach ( var filter in filters)
            {
                handlers= await filter.Filter(@event,handlers);
            }

            await Dispatcher<THandler, TProvider>.ExecuteEventHandlerAsync(serviceProvider,handlers, @event);
        }

        private static async Task ExecuteEventHandlerAsync(IServiceProvider serviceProvider,IEnumerable<THandler> handlers, IEvent<THandler> @event)
        {
            foreach (var handler in handlers)
            {
                @event.Handler = handler;
                await handler.ExecuteAction(serviceProvider, @event);
                if (!@event.Success)
                    break;
            }
        }
    }
}


