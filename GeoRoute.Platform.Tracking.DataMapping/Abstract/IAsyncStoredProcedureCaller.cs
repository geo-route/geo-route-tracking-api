using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoRoute.Platform.Tracking.DataMapping.Abstract;

public interface IAsyncStoredProcedureCaller : IDisposable
{
    Task<TValue> QueryFirstAsync<TValue>(string procedure, object[] args) where TValue : class;
    Task ExecuteQueryAsync(string procedure, object[] args);
    Task<IEnumerable<TValue>> QueryAsync<TValue>(string procedure, object[] args) where TValue : class;
}
