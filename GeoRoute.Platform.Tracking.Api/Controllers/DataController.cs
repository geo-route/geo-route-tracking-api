using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.Data.Egress;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;

using Microsoft.AspNetCore.Mvc;

namespace GeoRoute.Platform.Tracking.Api.Controllers;

[ApiController]
[Route("geo-route/tracking/[controller]")]
public class DataController : BaseController
{
    private readonly ILogger<IngressController> _logger;
    private readonly IDataRepository _dataRepository;

    public DataController(IDataRepository dataRepository, ITrackingRepository trackingRepository, ILogger<IngressController> logger) : base(trackingRepository)
    {
        this._logger = logger;
        this._dataRepository = dataRepository;
    }

    [HttpGet("sources/{source}/metrics/{metric}/measurements")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1568:Parameter value should not be overwritten in method body", Justification = "Default value.")]
    public async Task<IActionResult> GetAsync([FromRoute] int source, [FromRoute] string metric, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
	    start ??= new DateTime(1800, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        end ??= DateTime.MaxValue;

        var measurementSource = await this.GetSourceAsync(source);
        var measurementMetric = await this.GetMetricAsync(metric);
        this._logger.LogInformation("Loading {metricName} from {sourceName} between {start} and {end}", measurementMetric.Name, measurementSource.Name, start, end);

        var results = this._dataRepository.GetMeasurements(measurementSource, measurementMetric, start.GetValueOrDefault(), end.GetValueOrDefault());

        return this.Ok(new HttpResult<IEnumerable<Measurement>> {
	        Data = results,
            Id = this.GetRequestId()
        });
    }
}
