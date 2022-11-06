namespace GeoRoute.Platform.Tracking.Data.Dto;

public class Metric
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string Slug { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
}
