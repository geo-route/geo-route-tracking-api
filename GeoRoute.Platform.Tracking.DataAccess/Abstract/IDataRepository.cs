using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.Data.Egress;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IDataRepository
{
    IEnumerable<Measurement> GetMeasurements(Source source, Metric metric, DateTime start, DateTime end);
}
