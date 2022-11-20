using GeoRoute.Platform.Tracking.DataAccess.Models;
using GeoRoute.Platform.Tracking.DataMapping.Attributes;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IWaypointContext : IDisposable
{
    [ProcedureName("TrackingApi_SelectWaypointsByRoute")]
    Task<IEnumerable<Waypoint>> GetWaypointsAsync(int routeId);
    [ProcedureName("TrackingApi_SelectWaypointsById")]
    Task<Waypoint?> GetWaypointById(int id);
}
