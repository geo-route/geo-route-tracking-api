using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.Data.Ingress;

using AverageAggregateRequest = GeoRoute.Platform.Tracking.Data.Dto.AverageAggregateRequest;
using Source = GeoRoute.Platform.Tracking.Data.Dto.Source;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface ITrackingRepository
{
    Task<Source?> GetSourceAsync(int id);
    Task<Metric?> GetMetricAsync(string slug);
    Task CreateMeasurementAsync(Measurement measurement);
    ValueTask<decimal> ComputeAverageAsync(AverageAggregateRequest request);
}
