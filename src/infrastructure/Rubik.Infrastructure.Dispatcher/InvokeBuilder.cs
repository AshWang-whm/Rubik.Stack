using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;

namespace Rubik.Infrastructure.Dispatcher
{
    public delegate Task TaskInvokeDelegate(object target, params object[] parameters);

    public delegate void VoidInvokeDelegate(object target, object[] parameters);
    internal static class InvokeBuilder
    {
        public static TaskInvokeDelegate Build(MethodInfo methodInfo, Type targetType)
        {
            // Parameters to executor
            var targetParameter = Expression.Parameter(typeof(object), "target");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            var parameters = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (var i = 0; i < paramInfos.Length; i++)
            {
                var paramInfo = paramInfos[i];
                var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }

            // Call method
            var instanceCast = Expression.Convert(targetParameter, targetType);
            var methodCall = Expression.Call(instanceCast, methodInfo, parameters);

            // methodCall is "((Ttarget) target) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<VoidInvokeDelegate>(methodCall, targetParameter, parametersParameter);
                var voidExecutor = lambda.CompileFast();
                return delegate (object target, object[] paras)
                {
                    voidExecutor(target, paras);
                    return Task.CompletedTask;
                };
            }
            else if (methodCall.Type == typeof(Task))
            {
                // must coerce methodCall to match ActionExecutor signature
                var castMethodCall = Expression.Convert(methodCall, typeof(Task));
                var lambda = Expression.Lambda<TaskInvokeDelegate>(castMethodCall, targetParameter, parametersParameter);
                return lambda.CompileFast();
            }
            else
            {
                throw new NotSupportedException($"The return type of the [{methodInfo.Name}] method must be Task or void");
            }
        }
    }
}
