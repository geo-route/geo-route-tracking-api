using GeoRoute.Platform.Tracking.Data.Dto;

namespace GeoRoute.Platform.Tracking.Data.Egress;

public record Measurement
{
    public DateTime Timestamp { get; init; }
    public decimal Value { get; init; }
    public Location? Location { get; init; }
    public string? ExternalReference { get; init; }
}
