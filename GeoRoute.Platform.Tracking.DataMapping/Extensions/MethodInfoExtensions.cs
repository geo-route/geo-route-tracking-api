using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GeoRoute.Platform.Tracking.DataMapping.Extensions;

public static class MethodInfoExtensions
{
    public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
    {
        var awaitableObject = (Task?)methodInfo.Invoke(obj, parameters);

        if(awaitableObject == null) {
            throw new InvalidOperationException("Method is not awaitable");
        }

        await awaitableObject;
        var resultProperty = awaitableObject.GetType().GetProperty("Result");
        var result = resultProperty?.GetValue(awaitableObject);

        return result ?? Task.CompletedTask;
    }
}
