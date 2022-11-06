using GeoRoute.Platform.Tracking.Data.Dto;

namespace GeoRoute.Platform.Tracking.Data.Ingress;

public record ApproximateLocation
{
    public int SourceId { get; init; }
    public Location? Location { get; init; } = default;
	public decimal Accuracy { get; init; } = default;
}
