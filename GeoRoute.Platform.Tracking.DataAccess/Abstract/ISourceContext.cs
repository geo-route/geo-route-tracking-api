using GeoRoute.Platform.Tracking.DataAccess.Models;
using GeoRoute.Platform.Tracking.DataMapping.Attributes;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface ISourceContext : IDisposable
{
    
    [ProcedureName("TrackingApi_SelectSourceById")]
    Task<Source?> GetSourceAsync(int sourceId);
}
