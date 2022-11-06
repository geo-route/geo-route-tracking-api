namespace GeoRoute.Platform.Tracking.Data.Dto;

public record Source
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public DateTime Timestamp { get; init; }
}
