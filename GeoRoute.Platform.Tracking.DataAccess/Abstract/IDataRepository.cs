using GeoRoute.Platform.Tracking.Data.Dto;
using Measurement = GeoRoute.Platform.Tracking.Data.Ingress.Measurement;

namespace GeoRoute.Platform.Tracking.DataAccess.Abstract;

public interface IDataRepository
{
    IEnumerable<Measurement> GetMeasurements(Source source, Metric metric, DateTime start, DateTime end);
}
