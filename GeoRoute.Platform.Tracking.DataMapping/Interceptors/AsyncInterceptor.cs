using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using GeoRoute.Platform.Tracking.DataMapping.Abstract;
using GeoRoute.Platform.Tracking.DataMapping.Extensions;

namespace GeoRoute.Platform.Tracking.DataMapping.Interceptors;

public sealed class AsyncInterceptor : BaseInterceptor, IDisposable
{
    private readonly IAsyncStoredProcedureCaller _spCaller;
    private bool _disposed;

    public AsyncInterceptor(IAsyncStoredProcedureCaller spCaller)
    {
        this._spCaller = spCaller;
        this._disposed = false;
    }

    public override void Intercept(IInvocation invocation)
    {
        this.CheckDisposed();

        if(invocation.Method.Name == "Dispose") {
            this.Dispose();
        } else {
            this.InternalIntercept(invocation);
        }
    }

    private void InternalIntercept(IInvocation invocation)
    {
        var returnTypeGenericArguments = invocation.Method.ReturnType.GetGenericArguments();

        if(returnTypeGenericArguments.Length > 0) {
            this.InterceptGenericCall(invocation);
        } else {
            this.InterceptCall(invocation);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Framework", "AV2235:Call to Task.ContinueWith should be replaced with an await expression", Justification = "Cannot automatically marshall the result back to the caller. Do it manually.")]
    private void InterceptCall(IInvocation invocation)
    {
        var tcs = new TaskCompletionSource();
        invocation.ReturnValue = tcs.Task;

        this.InterceptAsync(invocation).ContinueWith(task => {
            if(task.IsFaulted) {
                tcs.SetException(task.Exception?.InnerException ?? new InvalidOperationException("Unable to complete the operation"));
            } else {
                tcs.SetResult();
            }
        });
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Framework", "AV2235:Call to Task.ContinueWith should be replaced with an await expression", Justification = "Cannot automatically marshall the result back to the caller. Do it manually.")]
    private void InterceptGenericCall(IInvocation invocation)
    {
        var returnTypeGenericArguments = invocation.Method.ReturnType.GetGenericArguments();
        var taskSourceType = typeof(TaskCompletionSource<>).MakeGenericType(returnTypeGenericArguments[0]);
        var tcs = Activator.CreateInstance(taskSourceType);
        invocation.ReturnValue = taskSourceType.GetProperty("Task")?.GetValue(tcs, null);

        this.InterceptAsync(invocation).ContinueWith(task => {
            InternalInterceptGenericCall(invocation, task, taskSourceType, tcs);
        });
    }

    private static void InternalInterceptGenericCall(IInvocation invocation, Task<object> task, Type taskSourceType, object? tcs)
    {
        var returnTypeGenericArguments = invocation.Method.ReturnType.GetGenericArguments();

        if(task.IsFaulted) {
            var method = taskSourceType.GetMethod("SetException", new[] { typeof(Exception) });
            method?.Invoke(tcs, new object[] { task.Exception?.InnerException! });
        } else {
            SetGenericResults(task, taskSourceType, tcs, returnTypeGenericArguments[0]);
        }
    }

    private static void SetGenericResults(Task<object> task, Type taskSourceType, object? tcs, Type returnType)
    {
        var result = task.Result;

        if(result.GetType() == returnType) {
            taskSourceType.GetMethod("SetResult")?.Invoke(tcs, new[] { result });
        } else {
            taskSourceType.GetMethod("SetResult")?.Invoke(tcs, new object?[] { null });
        }
    }

    protected override MethodInfo CreateTargetMethod(IInvocation invocation)
    {
        MethodInfo? method;
        var genericTypeArguments = invocation.Method.ReturnType.GetGenericArguments();

        if(genericTypeArguments.Length > 0) {
            method = this.CreateTargetMethodWithReturn(genericTypeArguments);
        } else {
            method = this._spCaller.GetType().GetMethod(nameof(this._spCaller.ExecuteQueryAsync));
        }

        if(method == null) {
            throw new InvalidOperationException("No target method has been found");
        }

        return method;
    }

    private MethodInfo CreateTargetMethodWithReturn(IReadOnlyList<Type> genericTypeArguments)
    {
        MethodInfo? method;

        if(typeof(IEnumerable).IsAssignableFrom(genericTypeArguments[0])) {
            var enumerableTypeArgument = genericTypeArguments[0].GenericTypeArguments[0];
            method = this._spCaller.GetType()
                .GetMethod(nameof(this._spCaller.QueryAsync))?
                .MakeGenericMethod(enumerableTypeArgument);
        } else {
            method = this._spCaller.GetType()
                .GetMethod(nameof(this._spCaller.QueryFirstAsync))?
                .MakeGenericMethod(genericTypeArguments[0]);
        }

        if(method == null) {
            throw new InvalidOperationException("Method not found");
        }

        return method;
    }

    private async Task<object> InterceptAsync(IInvocation invocation)
    {
        var arguments = new object[invocation.Arguments.Length * 2];
        var method = this.BuildMethodCall(invocation, arguments);
        var procedureName = GetProcedureName(invocation);
        var result = await method.InvokeAsync(this._spCaller, procedureName, arguments);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckDisposed()
    {
        if(this._disposed) {
            throw new ObjectDisposedException(nameof(AsyncInterceptor));
        }
    }

    public void Dispose()
    {
        if(this._disposed) {
            return;
        }

        this._disposed = true;
        this._spCaller.Dispose();
    }
}
