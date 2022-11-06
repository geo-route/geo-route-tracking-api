namespace GeoRoute.Platform.Tracking.Data.Dto;

public record AverageAggregateRequest
{
	public Metric Metric { get; init; } = default!;
	public Source Source { get; init; } = default!;
    public GroupingInterval Interval { get; init; }
    public DateTime StartTimestamp { get; init; }
    public DateTime EndTimestamp { get; init; }
}
