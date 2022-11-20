using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.DataAccess.Models;

[UsedImplicitly]
public record Waypoint
{
	public int Id { get; init; }
	public int RouteId { get; init; }
	public decimal Latitude { get; init; }
	public decimal Longitude { get; init; }
	public decimal MinimumProximity { get; init; }
	public int Order { get; init; }
}
