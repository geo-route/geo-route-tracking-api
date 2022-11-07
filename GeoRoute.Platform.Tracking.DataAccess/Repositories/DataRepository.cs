using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.Data.Ingress;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;
using Microsoft.Extensions.Logging;

namespace GeoRoute.Platform.Tracking.DataAccess.Repositories;

public class DataRepository : IDataRepository
{
    private readonly ILogger<DataRepository> _logger;
    private readonly IDataContext _context;

    public DataRepository(IDataContext context, ILogger<DataRepository> logger)
    {
        this._context = context;
        this._logger = logger;
    }

    public IEnumerable<Measurement> GetMeasurements(Source source, Metric metric, DateTime start, DateTime end)
    {
        throw new NotImplementedException();
    }
}
