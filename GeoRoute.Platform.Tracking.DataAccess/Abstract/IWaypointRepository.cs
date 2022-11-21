using GeoRoute.Platform.Tracking.Data.Dto;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IWaypointRepository
{
	Task<IEnumerable<Waypoint>> GetWaypointsByRouteAsync(int routeId);
	Task<Waypoint> GetWaypointAsync(int id);
}
