using System.Net;

namespace GeoRoute.Platform.Tracking.Api.Exceptions;

public class HttpException : Exception
{
    public HttpStatusCode StatusCode { get; init; }

    protected HttpException(string message, HttpStatusCode statusCode) : base(message)
    {
        this.StatusCode = statusCode;
    }
}
