namespace GeoRoute.Platform.Tracking.Data.Dto;

public record Waypoint
{
	public int Id { get; init; }
    public int RouteId { get; init; }
    public Location Coordinates { get; init; } = new Location();
    public decimal MinimumProximity { get; init; }
    public int Order { get; init; }
}
