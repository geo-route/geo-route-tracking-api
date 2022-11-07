using GeoRoute.Platform.Tracking.DataAccess.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace GeoRoute.Platform.Tracking.Api.Controllers;

[ApiController]
[Route("geo-route/tracking/[controller]")]
public class DataController : BaseController
{
    private readonly ILogger<IngressController> _logger;

    public DataController(ITrackingRepository repository, ILogger<IngressController> logger) : base(repository)
    {
        this._logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromRoute] int sourceId, [FromRoute] string metric,
                                              [FromRoute] DateTime? start, [FromRoute] DateTime? end)
    {
	    start ??= DateTime.MinValue;
	    end ??= DateTime.MaxValue;

	    var source = await this.GetSourceAsync(sourceId);
	    var measurementMetric = await this.GetMetric(metric);



	    return this.Ok();
    }
}
