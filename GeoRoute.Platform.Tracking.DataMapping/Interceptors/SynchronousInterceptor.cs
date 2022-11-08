using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

using Castle.DynamicProxy;
using GeoRoute.Platform.Tracking.DataMapping.Abstract;

namespace GeoRoute.Platform.Tracking.DataMapping.Interceptors;

public sealed class SynchronousInterceptor : BaseInterceptor, IDisposable
{
    private readonly IStoredProcedureCaller _spCaller;
    private bool _disposed;

    public SynchronousInterceptor(IStoredProcedureCaller spCaller)
    {
        this._spCaller = spCaller;
        this._disposed = false;
    }

    public override void Intercept(IInvocation invocation)
    {
        if(invocation.Method.Name == "Dispose") {
            this.Dispose();
        } else {
            this.InternalIntercept(invocation);
        }
    }

    private void InternalIntercept(IInvocation invocation)
    {
        this.CheckDisposed();
        var procedureName = GetProcedureName(invocation);

        var arguments = invocation.Arguments.Length > 0 ? new object[invocation.Arguments.Length * 2] : Array.Empty<object>();
        var method = this.BuildMethodCall(invocation, arguments);

        if(HasTargetReturnType(invocation)) {
            invocation.ReturnValue = method.Invoke(this._spCaller, new object[] { procedureName, arguments });
        } else {
            method.Invoke(this._spCaller, new object[] { procedureName, arguments });
        }
    }

    protected override MethodInfo CreateTargetMethod(IInvocation invocation)
    {
        MethodInfo? method;

        if(HasTargetReturnType(invocation)) {
            method = this.CreateTargetMethodWithReturnType(invocation);
        } else {
            method = this._spCaller.GetType().GetMethod(nameof(this._spCaller.ExecuteStoredProcedure));
        }

        if(method == null) {
            throw new InvalidOperationException("No target method has been found");
        }

        return method;
    }

    private static bool HasTargetReturnType(IInvocation invocation)
    {
        return invocation.Method.ReturnType != typeof(void);
    }

    private MethodInfo CreateTargetMethodWithReturnType(IInvocation invocation)
    {
        MethodInfo? method;

        if(typeof(IEnumerable).IsAssignableFrom(invocation.Method.ReturnType)) {
            method = this._spCaller.GetType()
                .GetMethod(nameof(this._spCaller.Stream))?
                .MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0]);
        } else {
            method = this._spCaller.GetType()
                .GetMethod(nameof(this._spCaller.QueryFirst))?
                .MakeGenericMethod(invocation.Method.ReturnType);
        }

        if(method == null) {
            throw new InvalidOperationException("No target method has been found");
        }

        return method;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckDisposed()
    {
        if(this._disposed) {
            throw new ObjectDisposedException(nameof(SynchronousInterceptor));
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
