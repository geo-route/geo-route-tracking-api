using System.Net;

using GeoRoute.Platform.Tracking.Api.Exceptions;
using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.Data.Ingress;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;

using Microsoft.AspNetCore.Mvc;

namespace GeoRoute.Platform.Tracking.Api.Controllers;

[ApiController]
[Route("geo-route/v1/tracking/[controller]")]
public class IngressController : BaseController
{
    private readonly ILogger<IngressController> _logger;

    public IngressController(ITrackingRepository repository, ILogger<IngressController> logger) : base(repository)
    {
        this._logger = logger;
    }

    [HttpPost("location")]
    public async Task<IActionResult> CreateLocationLogAsync([FromBody] ApproximateLocation approximateLocation)
    {
        var id = this.GetRequestId();

        using var scope = this._logger.BeginScope(new Dictionary<string, object> { ["SourceId"] = approximateLocation.SourceId });
        await this.InternalCreateLocationLogAsync(approximateLocation);

        return this.Ok(new HttpResult<string> {
            Data = "Location has been accepted",
            Id = id
        });
    }

    private async Task InternalCreateLocationLogAsync(ApproximateLocation approximateLocation)
    {
        var sourceTask = this.GetSourceAsync(approximateLocation.SourceId);
        var metricTask = this.GetDirectionalMetricAsync();

        if(approximateLocation.Location == null) {
            this._logger.LogError("Unable to create measurement: location was not provided");
            throw new InvalidInputException("No valid location provided", HttpStatusCode.BadRequest);
        }

        await Task.WhenAll(sourceTask, metricTask);
        await this.CreateMeasurementAsync(sourceTask.Result, metricTask.Result, approximateLocation);
    }

    private async Task CreateMeasurementAsync(Source source, Metric metric, ApproximateLocation approximateLocation)
    {
        var measurement = new Measurement {
            Metric = metric,
            Source = source,
            Value = approximateLocation.Accuracy,
            Timestamp = DateTime.UtcNow,
            Location = approximateLocation.Location
        };

        this._logger.LogInformation("Logging {metricName} from {sourceName}", metric.Name, source.Name);
        await this._trackingRepository.CreateMeasurementAsync(measurement).ConfigureAwait(false);
    }
}
