using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rubik.Infrastructure.Dispatcher
{
    public abstract class Event : IEvent
    {
        public EventHandlerAttribute? Handler { get; set;}
        public bool Success { get; set; }
        public Exception? Exception { get; set; }
    }

    public abstract class Event<TEventParameter> : Event where TEventParameter:class,new()
    {
        public TEventParameter EventParameter { get; set; } = new();
    }


    public abstract class GenericsEvent<THanlder> : IEvent<THanlder>
        where THanlder : EventHandlerAttribute
    {
        public virtual bool Success { get; set; } = true;
        public Exception? Exception { get; set; }
        public THanlder? Handler { get; set; }

    }
}
