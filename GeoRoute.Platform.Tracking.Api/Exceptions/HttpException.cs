using System.Net;
using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.Api.Exceptions;

[PublicAPI]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "No need.")]
public class HttpException : Exception
{
    public HttpStatusCode StatusCode { get; init; }

    protected HttpException(string message, HttpStatusCode statusCode) : base(message)
    {
        this.StatusCode = statusCode;
    }
}
