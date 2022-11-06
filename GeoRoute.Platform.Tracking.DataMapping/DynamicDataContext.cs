using System;
using Castle.DynamicProxy;
using GeoRoute.Platform.Tracking.DataMapping.Interceptors;
using GeoRoute.Platform.Tracking.DataMapping.Services;

namespace GeoRoute.Platform.Tracking.DataMapping;

public static class DynamicDataContext
{
    /// <summary>
    /// Create a new dynamic data context that executes stored procedures in an synchronous manner.
    /// </summary>
    /// <typeparam name="TInterfaceType">Interface type.</typeparam>
    /// <param name="connectionString">Connection string to use.</param>
    /// <returns>An instance of <typeparamref name="TInterfaceType"/>, connecting to <paramref name="connectionString"/>.</returns>
    public static TInterfaceType Create<TInterfaceType>(string connectionString) where TInterfaceType : class, IDisposable
    {
        var generator = new ProxyGenerator();
        var service = new StoredProcedureCallerService(new MSSqlConnectionService(), connectionString);
        var interceptor = new SynchronousInterceptor(service);

        return generator.CreateInterfaceProxyWithoutTarget<TInterfaceType>(interceptor);
    }
}
