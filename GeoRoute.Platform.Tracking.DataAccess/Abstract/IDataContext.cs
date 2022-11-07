using GeoRoute.Platform.Tracking.DataAccess.Models;
using GeoRoute.Platform.Tracking.DataMapping.Attributes;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IDataContext : IDisposable
{

    [ProcedureName("TrackingApi_SelectMeasurements")]
    Task<IEnumerable<Measurement>> GetMetricAsync(string slug);
}
