using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.Dispatcher.Internal
{
    public interface IEventHandlerFilter<TEvent,THandler>
         where THandler : EventHandlerAttribute
        where TEvent:IEvent<THandler>
    {
        /// <summary>
        /// Handlers 过滤器
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="event"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        Task<List<THandler>> Filter(TEvent @event, List<THandler> handlers);
    }

}
