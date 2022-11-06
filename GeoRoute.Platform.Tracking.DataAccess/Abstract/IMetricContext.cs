using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.DataMapping.Attributes;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IMetricContext : IDisposable
{
    [ProcedureName("TrackingApi_SelectMetricBySlug")]
    Task<Metric?> GetMetricAsync(string slug);
}
