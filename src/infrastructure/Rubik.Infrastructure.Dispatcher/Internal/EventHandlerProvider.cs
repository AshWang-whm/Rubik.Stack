using System.Reflection;

namespace Rubik.Infrastructure.Dispatcher.Internal
{
    public abstract class EventHandlerProvider<THandler>
        where THandler : EventHandlerAttribute
    {
        public EventHandlerProvider()
        {
            Build();
        }

        Dictionary<Type, List<THandler>> EventHandlerDictionary { get; set; }=new Dictionary<Type, List<THandler>>();
        internal abstract bool BuildHandler(Type type, MethodInfo methodInfo, out THandler? handler);

        public bool TryGetValue(Type type, out List<THandler> handlers)
        {
            return EventHandlerDictionary.TryGetValue(type, out handlers!);
        }

        public IEnumerable<List<THandler>> GetAllValues()
        {
            foreach (var item in EventHandlerDictionary)
            {
                yield return item.Value;
            }
        }

        internal virtual Dictionary<Type, List<THandler>> BuildDictionary()
        {
            var dict =new Dictionary<Type, List<THandler>>();
            var types = AppDomain.CurrentDomain.GetAssemblies().Distinct().SelectMany(a => a.GetTypes()).ToArray();

            // event , handler mapping
            foreach (var type in types)
            {
                if (!type.IsConcrete())
                    continue;
                foreach (var method in type.GetMethods())
                {
                    if(BuildHandler(type,method,out var handler))
                    {
                        AddEventHandlerItem(handler);
                    }
                }
            }

            return dict;
        }

        void AddEventHandlerItem(THandler? handler)
        {
            if (handler == null)
                return;
            var keyType = handler.ParameterTypes![0]!;
            if (!EventHandlerDictionary!.ContainsKey(keyType))
            {
                EventHandlerDictionary.Add(keyType, new List<THandler>());
            }

            if (!EventHandlerDictionary[keyType].Any(x => x.ActionMethodInfo.Equals(handler.ActionMethodInfo) && x.InstanceType == handler.InstanceType))
            {
                EventHandlerDictionary[keyType].Add(handler);
            }
        }

        void Build()
        {
            BuildDictionary() ;
            // sort
            EventHandlerDictionary = EventHandlerDictionary.ToDictionary(
                dispatchRelative => dispatchRelative.Key,
                dispatchRelative => dispatchRelative.Value.OrderBy(attr => attr.Order).ToList()
            );
        }

        internal IEnumerable<Type> GetAddServiceTypeList() => EventHandlerDictionary!
            .SelectMany(relative => relative.Value)
            .Where(dispatchHandler => dispatchHandler.InvokeDelegate != null)
            .Select(dispatchHandler => dispatchHandler.InstanceType!).Distinct();
    }

}
