using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.Data.Ingress;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;

using AverageAggregateRequest = GeoRoute.Platform.Tracking.Data.Dto.AverageAggregateRequest;

namespace GeoRoute.Platform.Tracking.DataAccess.Repositories;

public class TrackingRepository : ITrackingRepository
{
    private readonly IMetricContext _metrics;
    private readonly ISourceContext _sources;
    private readonly IMeasurementContext _measurements;

    public TrackingRepository(IMetricContext metrics, ISourceContext sources, IMeasurementContext measurements)
    {
        this._metrics = metrics;
        this._sources = sources;
        this._measurements = measurements;
    }

    public async Task<Source?> GetSourceAsync(int id)
    {
        var source = await this._sources.GetSourceAsync(id).ConfigureAwait(false);

        if (source == null) {
            throw new InvalidOperationException($"Source with ID {id} does not exist");
        }

        return new Source {
            Description = source.Description,
            Id = source.Id,
            Name = source.Name,
            Timestamp = source.Timestamp
        };
    }

    public async Task<Metric?> GetMetricAsync(string slug)
    {
        return await this._metrics.GetMetricAsync(slug).ConfigureAwait(false);
    }

    public async Task CreateMeasurementAsync(Measurement measurement)
    {
        await this._measurements.CreateMeasurementAsync(
            measurement.Source.Id,
            measurement.Metric.Id,
            measurement.Value,
            measurement.Location?.Latitude,
            measurement.Location?.Longitude,
            measurement.ExternalReference,
            measurement.Timestamp).ConfigureAwait(false);
    }

    public async ValueTask<decimal> ComputeAverageAsync(AverageAggregateRequest request)
    {
	    var result = request.Interval switch {
		    GroupingInterval.Day => throw new ArgumentOutOfRangeException(),
		    GroupingInterval.Hour => await this._measurements.ComputeAverageByHourAsync(request.Source.Id, request.Metric.Id),
		    GroupingInterval.Minute => throw new ArgumentOutOfRangeException(),
		    GroupingInterval.Second => throw new ArgumentOutOfRangeException(),
		    _ => throw new ArgumentOutOfRangeException()
	    };

	    return result.AverageValue;
    }
}
