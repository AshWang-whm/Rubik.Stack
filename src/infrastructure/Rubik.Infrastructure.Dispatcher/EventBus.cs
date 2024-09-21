using Rubik.Infrastructure.Dispatcher.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Rubik.Infrastructure.Dispatcher
{
    public class EventBus : IEventBus
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Internal.Dispatcher dispatcher;
        public EventBus(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            dispatcher = serviceProvider.GetRequiredService<Internal.Dispatcher>();
        }

        public async Task PublishAsync(IEvent @event)
        {
            var scope = serviceProvider.CreateScope().ServiceProvider;
            var middlewares = scope.GetRequiredService<IEnumerable<IEventMiddleware>>();
            EventHandlerDelegate eventHandlerDelegate = async () =>
            {
                await dispatcher.PublishEventAsync(scope, @event);
            };

            await middlewares.Aggregate(eventHandlerDelegate, (next, middleware) => () => middleware.HandleAsync(@event, next)).Invoke();
        }

    }

    public class EventBus<THandler,TProvider> : IEventBus<THandler>
        where THandler : EventHandlerAttribute
        where TProvider:EventHandlerProvider<THandler>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Dispatcher<THandler, TProvider> dispatcher;

        public EventBus(IServiceProvider serviceProvider,Dispatcher<THandler, TProvider> dispatcher)
        {
            this.serviceProvider = serviceProvider;
            this.dispatcher = dispatcher;
        }

        public async Task PublishAsync<TEvent>(TEvent @event)
            where TEvent:IEvent<THandler>
        {
            var middlewares = serviceProvider.GetRequiredService<IEnumerable<IGenericsEventMiddleware<TEvent, THandler>>>();
            EventHandlerDelegate eventHandlerDelegate = async () =>
            {
                await dispatcher.Publish(serviceProvider, @event);
            };
            await middlewares.Aggregate(eventHandlerDelegate,(next,middleware)=>()=>middleware.HandleAsync(@event,next)).Invoke();
        }
    }
}
