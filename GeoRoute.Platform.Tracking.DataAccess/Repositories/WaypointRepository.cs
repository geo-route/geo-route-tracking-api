using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;

namespace GeoRoute.Platform.Tracking.DataAccess.Repositories;

public class WaypointRepository : IWaypointRepository
{
	private readonly IWaypointContext _context;

	public WaypointRepository(IWaypointContext context)
	{
		this._context = context;
	}

	public async Task<IEnumerable<Waypoint>> GetWaypointsByRouteAsync(int routeId)
	{
		var waypoints = await this._context.GetWaypointsAsync(routeId).ConfigureAwait(false);
		return waypoints.Select(wp => new Waypoint {
			Coordinates = new Location {
				Latitude = wp.Latitude,
				Longitude = wp.Longitude
			},
			Id = wp.Id,
			MinimumProximity = wp.MinimumProximity,
			Order = wp.Order,
			RouteId = wp.RouteId
		});
	}

	public async Task<Waypoint> GetWaypointAsync(int id)
	{
		var waypoint = await this._context.GetWaypointById(id).ConfigureAwait(false);

		if(waypoint == null) {
			throw new InvalidOperationException("Waypoint not found");
		}

		return new Waypoint {
			Coordinates = new Location {
				Latitude = waypoint.Latitude,
				Longitude = waypoint.Longitude
			},
			Id = waypoint.Id,
			MinimumProximity = waypoint.MinimumProximity,
			Order = waypoint.Order,
			RouteId = waypoint.RouteId
		};
	}
}
