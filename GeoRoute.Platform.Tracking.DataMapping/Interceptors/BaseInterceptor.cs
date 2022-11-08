using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.DynamicProxy;

using GeoRoute.Platform.Tracking.DataMapping.Attributes;

namespace GeoRoute.Platform.Tracking.DataMapping.Interceptors;

public abstract class BaseInterceptor : IInterceptor
{
    protected MethodInfo BuildMethodCall(IInvocation invocation, IList<object> arguments)
    {
        var method = this.CreateTargetMethod(invocation);
        var parameters = invocation.Method.GetParameters();

        for(var index = 0; index < parameters.Length; index++) {
            var name = index * 2;
            var value = index * 2 + 1;

            var argumentValue = GetArgumentValue(invocation, index, parameters);

            arguments[name] = parameters[index].Name!;
            arguments[value] = argumentValue;

        }

        return method;
    }

    protected static string GetProcedureName(IInvocation invocation)
    {
        var attr = invocation.Method.GetCustomAttribute<ProcedureNameAttribute>();
        var name = invocation.Method.Name;

        if(attr == null) {
            return name;
        }

        if(!string.IsNullOrEmpty(attr.Name)) {
            name = attr.Name;
        }

        return name;
    }

    private static object GetArgumentValue(IInvocation invocation, int index, IReadOnlyList<ParameterInfo> parameters)
    {
        var argumentValue = invocation.Arguments[index];

        if(argumentValue is IEnumerable enumType and not string) {
            var toList = typeof(Enumerable).GetMethod("ToList");
            var genericType = GetGenericTypeFromEnumerable(parameters[index]);
            var genericToList = toList?.MakeGenericMethod(genericType);

            argumentValue = genericToList?.Invoke(null, new object[] { enumType });
        }

        return argumentValue!;
    }

    private static Type GetGenericTypeFromEnumerable(ParameterInfo info)
    {
        var genericArguments = info.ParameterType.GenericTypeArguments;

        if(genericArguments.Length == 0) {
            throw new ArgumentException("Type has no generic arguments", nameof(info));
        }

        return genericArguments[0];
    }

    protected abstract MethodInfo CreateTargetMethod(IInvocation invocation);
    public abstract void Intercept(IInvocation invocation);
}
