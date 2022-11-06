using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.DataMapping.Attributes;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IMeasurementContext : IDisposable
{
    [ProcedureName("TrackingApi_InsertMeasurement")]
    Task CreateMeasurementAsync(int sourceId, int metricId, decimal value, decimal? lat, decimal? lon, string? externalReference, DateTime? timestamp);

    [ProcedureName("TrackingApi_SelectAverageByHour")]
    Task<AverageAggregateResult> ComputeAverageByHourAsync(int sourceId, int metricId);
}
