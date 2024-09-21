
namespace Rubik.Infrastructure.Dispatcher.Internal
{
    internal class DispatcherBase
    {
        protected static DispatchHandlerRelationShip DispatchHandlerRelationShip=new();

        public async Task PublishEventAsync(IServiceProvider serviceProvider,IEvent @event)
        {
            var eventType = @event.GetType();
            if (!DispatchHandlerRelationShip.HandlerDictionary.TryGetValue(eventType, out var handlers))
            {
                throw new Exception(
                    $"The {eventType.FullName} Handler method was not found.");
            }
            await ExecuteEventHandlerAsync(serviceProvider, handlers, @event);
        }

        private async Task ExecuteEventHandlerAsync(IServiceProvider serviceProvider,IEnumerable<EventHandlerAttribute> handlers,IEvent @event)
        {
            try
            {
                foreach (var handler in handlers)
                {
                    @event.Handler = handler;
                    await handler.ExecuteAction(serviceProvider, @event);
                    if (!@event.Success)
                        break;
                }
            }
            catch(Exception ex)
            {
                @event.Exception = ex;
                @event.Success = false;
            }
        }


        protected static void AddRelationShip(Type parameterType, EventHandlerAttribute handler)
        {
            DispatchHandlerRelationShip.Add(parameterType, handler);
        }

        protected static IEnumerable<Type> GetAddServiceTypeList() => DispatchHandlerRelationShip.HandlerDictionary!
            .SelectMany(relative => relative.Value)
            .Where(dispatchHandler => dispatchHandler.InvokeDelegate != null)
            .Select(dispatchHandler => dispatchHandler.InstanceType!).Distinct();

        protected static void Build() => DispatchHandlerRelationShip.Build();

    }


}


