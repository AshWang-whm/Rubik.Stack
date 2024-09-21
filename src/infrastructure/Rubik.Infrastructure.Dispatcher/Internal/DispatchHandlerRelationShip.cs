
namespace Rubik.Infrastructure.Dispatcher.Internal
{
    internal class DispatchHandlerRelationShip
    {
        public Dictionary<Type, List<EventHandlerAttribute>> HandlerDictionary { get; set; } = new Dictionary<Type, List<EventHandlerAttribute>>();

        public void Add(Type keyEventType, EventHandlerAttribute handler)
        {
            Add(keyEventType, handler, HandlerDictionary);
        }

        private static void Add(Type keyEventType,
            EventHandlerAttribute handlers,
            Dictionary<Type, List<EventHandlerAttribute>> dispatchRelativeNetwork)
        {
            if (!dispatchRelativeNetwork.ContainsKey(keyEventType))
            {
                dispatchRelativeNetwork.Add(keyEventType, new List<EventHandlerAttribute>());
            }

            if (!dispatchRelativeNetwork[keyEventType].Any(x => x.ActionMethodInfo.Equals(handlers.ActionMethodInfo) && x.InstanceType == handlers.InstanceType))
            {
                dispatchRelativeNetwork[keyEventType].Add(handlers);
            }
        }

        internal void Build()
        {
            // sort
            HandlerDictionary = HandlerDictionary.ToDictionary(
                dispatchRelative => dispatchRelative.Key,
                dispatchRelative => dispatchRelative.Value.OrderBy(attr => attr.Order).ToList()
            );
        }
    }

}

