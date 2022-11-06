using System;
using System.Collections.Generic;

namespace GeoRoute.Platform.Tracking.DataMapping.Abstract;

public interface IStoredProcedureCaller : IDisposable
{
    TValue QueryFirst<TValue>(string procedure, object[] args) where TValue : class;
    void ExecuteStoredProcedure(string procedure, object[] args);
    IEnumerable<TValue> Stream<TValue>(string procedure, object[] args) where TValue : class;
}
