namespace GeoRoute.Platform.Tracking.Data.Dto;

public record AverageAggregateResult
{
    public DateTime Timestamp { get; init; }
    public decimal AverageValue { get; init; }
    public decimal StandardDeviation { get; init; }
}
