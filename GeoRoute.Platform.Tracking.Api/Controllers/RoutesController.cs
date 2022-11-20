using GeoRoute.Platform.Tracking.Api.Abstract;
using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.Data.Egress;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace GeoRoute.Platform.Tracking.Api.Controllers;

[ApiController]
[Route("geo-route/v1/tracking/[controller]")]
public sealed class RoutesController : BaseController
{
	private readonly ILogger<RoutesController> _logger;
	private readonly IWaypointRepository _repository;
	private readonly IGeoService _geoService;

	public RoutesController(IWaypointRepository waypointRepository, IGeoService geoService, ITrackingRepository repository, ILogger<RoutesController> logger) : base(repository)
	{
		this._logger = logger;
		this._geoService = geoService;
		this._repository = waypointRepository;
	}

	[HttpPost("{id}/nearby/{id}")]
	public async Task<IActionResult> GetAsync([FromRoute] int id, [FromBody] Location location)
	{
		using var _ = this._logger.BeginScope(new Dictionary<string, object> { ["WaypointId"] = id, });
        this._logger.LogDebug("Trying to lookup waypoint with ID {id}", id);

        var wp = await this._repository.GetWaypointAsync(id).ConfigureAwait(false);
        var distance = this._geoService.GetDistance(wp.Coordinates, location);
        var isWithinProximity = Convert.ToDecimal(distance) > wp.MinimumProximity;

        return this.Ok(CreateProximityResult(isWithinProximity));
	}

	private HttpResult<ProximityResult> CreateProximityResult(bool success)
	{
		var result = new ProximityResult {
			WithinProximity = success,
			Result = success
				? "Location is within the minimum proximity"
				: "Location is not within the minimum proximity"
		};

		return new HttpResult<ProximityResult> {
			Data = result,
			Id = this.GetRequestId()
		};
	}
}
