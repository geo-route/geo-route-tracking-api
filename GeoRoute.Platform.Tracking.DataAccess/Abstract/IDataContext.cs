using GeoRoute.Platform.Tracking.DataAccess.Models;
using GeoRoute.Platform.Tracking.DataMapping.Attributes;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IDataContext : IDisposable
{

    [ProcedureName("TrackingApi_SelectMeasurements")]
    IEnumerable<Measurement> GetMeasurements(int sourceId, int metricId, DateTime start, DateTime end);
}
