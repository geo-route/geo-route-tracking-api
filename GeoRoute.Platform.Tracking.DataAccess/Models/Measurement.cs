namespace GeoRoute.Platform.Tracking.DataAccess.Models;

public record Measurement
{
    public DateTime Timestamp { get; init; }
    public int SourceId { get; init; }
    public int MetricId { get; init; }
    public decimal Value { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public string? ExternalReference { get; init; }
}
