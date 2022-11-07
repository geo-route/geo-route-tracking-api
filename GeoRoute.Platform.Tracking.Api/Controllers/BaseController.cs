using System.Net;
using GeoRoute.Platform.Tracking.Api.Exceptions;
using GeoRoute.Platform.Tracking.Data.Dto;
using GeoRoute.Platform.Tracking.DataAccess.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace GeoRoute.Platform.Tracking.Api.Controllers;

public class BaseController : Controller
{
    protected readonly ITrackingRepository _trackingRepository;

    protected BaseController(ITrackingRepository repository)
    {
        this._trackingRepository = repository;
    }

    protected async Task<Source> GetSourceAsync(int id)
    {
        var source = await this._trackingRepository.GetSourceAsync(id).ConfigureAwait(false);

        if(source == null) {
            throw new InvalidInputException("Source not found!", HttpStatusCode.UnprocessableEntity);
        }

        return source;
    }

    protected Guid GetRequestId()
    {
        return (Guid)this.HttpContext.Items["RequestId"]!;
    }

    protected async Task<Metric> GetDirectionalMetric()
    {
        var metric = await this._trackingRepository.GetMetricAsync("proximity").ConfigureAwait(false);

        if(metric == null) {
            throw new InvalidInputException("Metric not found!", HttpStatusCode.InternalServerError);
        }

        return metric;
    }

    protected async Task<Metric> GetMetric(string slug)
    {
        var metric = await this._trackingRepository.GetMetricAsync(slug).ConfigureAwait(false);

        if(metric == null) {
            throw new InvalidInputException("Metric not found!", HttpStatusCode.InternalServerError);
        }

        return metric;
    }
}
