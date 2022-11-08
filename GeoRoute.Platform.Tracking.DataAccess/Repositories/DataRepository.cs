using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;
using Microsoft.Extensions.Logging;

using Measurement = GeoRoute.Platform.Tracking.Data.Egress.Measurement;

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
	    return this._context.GetMeasurements(source.Id, metric.Id, start, end)
		    .Select(m => ConvertMeasurement(m, source, metric));
    }

    private static Measurement ConvertMeasurement(Models.Measurement measurement, Source source, Metric metric)
    {
	    Location? location = null;

	    if(measurement.Longitude != null && measurement.Latitude != null) {
		    location = new Location {
			    Latitude = measurement.Latitude.Value,
                Longitude = measurement.Longitude.Value
		    };
	    }

	    return new Measurement {
		    ExternalReference = measurement.ExternalReference,
		    Timestamp = measurement.Timestamp,
		    Value = measurement.Value,
            Location = location
	    };
    }
}
