
namespace Rubik.Infrastructure.Dispatcher
{
    public interface IEventBus
    {
        Task PublishAsync(IEvent @event);
    }

    public interface IEventBus<THandler>
        where THandler: EventHandlerAttribute
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent:IEvent<THandler>;
    }
}
