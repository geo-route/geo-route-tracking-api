using GeoRoute.Platform.Tracking.Data.Dto;
using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.Data.Ingress;

[PublicAPI]
public record AverageAggregateRequest
{
	public string Metric { get; init; } = default!;
    public int SourceId { get; init; }
    public GroupingInterval Interval { get; init; }
    public DateTime StartTimestamp { get; init; }
    public DateTime EndTimestamp { get; init; }
}
