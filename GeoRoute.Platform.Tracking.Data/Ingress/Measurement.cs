using GeoRoute.Platform.Tracking.Data.Dto;

namespace GeoRoute.Platform.Tracking.Data.Ingress;

public record Measurement
{
    public DateTime Timestamp { get; init; }
    public Source Source { get; init; } = default!;
    public Metric Metric { get; init; } = default!;
    public decimal Value { get; init; }
    public Location? Location { get; init; }
    public string? ExternalReference { get; init; }
}
