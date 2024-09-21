
namespace Rubik.Infrastructure.Dispatcher
{
     public interface IEventBase
    {
        bool Success { get; set; }
        Exception? Exception { get; set; }
    }

    /// <summary>
    /// 默认EventHandlerAttribute的事件
    /// </summary>
    public interface IEvent: IEventBase
    {
        EventHandlerAttribute? Handler { get; set; }
    }

    /// <summary>
    /// 自定义HandlerAttribute
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    public interface IEvent<THandler>: IEventBase
        where THandler : EventHandlerAttribute
    {
        THandler? Handler { get; set; }
    }

    public delegate Task EventHandlerDelegate();

    /// <summary>
    /// Middleware
    /// </summary>
    public interface IEventMiddleware
    {
        Task HandleAsync(IEvent @event, EventHandlerDelegate next);
    }

    /// <summary>
    /// 自定义的Middleware
    /// </summary>
    /// <typeparam name="TEvent">自定义事件</typeparam>
    /// <typeparam name="THandler">自定义Handler</typeparam>
    public interface IGenericsEventMiddleware<TEvent,THandler>
        where THandler : EventHandlerAttribute
        where TEvent:IEvent<THandler>
    {
        Task HandleAsync(TEvent @event, EventHandlerDelegate next);

    }
}



