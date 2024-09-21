using Rubik.Infrastructure.Dispatcher.Internal;
using System.Reflection;

namespace Rubik.Infrastructure.Dispatcher.Default
{
    /// <summary>
    /// 自定义的泛型Handler Mapping
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    public class DefaultEventHandlerProvider<THandler>
        : EventHandlerProvider<THandler>
        where THandler: EventHandlerAttribute, new()
    {
        internal override bool BuildHandler(Type type, MethodInfo methodInfo,out THandler? handler)
        {
            try
            {
                handler = methodInfo.GetCustomAttribute<THandler>();
                if(handler != null)
                {
                    var parameters = methodInfo.GetParameters().Where(a=>typeof(IEvent<THandler>).IsAssignableFrom(a.ParameterType)).ToList();
                    if (parameters.Count != 1)
                        return false;
                    handler.InstanceType = type;
                    // only allow one parameter and the first one will be the key event type
                    handler.EventType = parameters[0].ParameterType;
                    handler.ActionMethodInfo = methodInfo;
                    handler.BuildExpression();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fail to Build Handler,type name:[{type.FullName}],method:[{methodInfo.Name}].{ex.Message}");
            }
            return false;
        }
    }
}
