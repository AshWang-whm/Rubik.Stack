using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Rubik.Infrastructure.Dispatcher
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute : Attribute
    {
        public int Order { get; set; }
        private MethodInfo? _actionMethodInfo;
        internal MethodInfo? ActionMethodInfo
        {
            get => _actionMethodInfo;
            set
            {
                var parameters = value!.GetParameters();
                Type[] types = new Type[parameters.Length];

                for (int index = 0; index < types.Length; index++)
                {
                    types[index] = parameters[index].ParameterType;
                }
                ParameterTypes = types;
                _actionMethodInfo = value;
            }
        }
        internal Type[]? ParameterTypes { get; set; }
        internal Type? InstanceType { get; set; }
        internal Type? EventType { get; set; }
        internal TaskInvokeDelegate? InvokeDelegate { get;  set; }
        internal void BuildExpression()
        {
            InvokeDelegate = InvokeBuilder.Build(ActionMethodInfo!, InstanceType!);
        }
        internal async Task ExecuteAction(IServiceProvider serviceProvider, IEventBase @event)
        {
            await InvokeDelegate!.Invoke(serviceProvider.GetRequiredService(InstanceType!),
                GetParameters(serviceProvider, @event));
        }
        internal object[] GetParameters(IServiceProvider serviceProvider, IEventBase @event)
        {
            var parameters = new object[ParameterTypes!.Length];
            for (int index = 0; index < ParameterTypes.Length; index++)
            {
                if (ParameterTypes[index] == @event.GetType())
                {
                    parameters[index] = @event;
                }
                else
                {
                    parameters[index] = serviceProvider.GetService(ParameterTypes[index])!;
                }
            }
            return parameters;
        }
    }
}
