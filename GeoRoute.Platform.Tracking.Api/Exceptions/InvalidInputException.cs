using System.Net;

namespace GeoRoute.Platform.Tracking.Api.Exceptions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "No need.")]
public class InvalidInputException : HttpException
{
    public InvalidInputException(string message, HttpStatusCode statusCode) : base(message, statusCode)
    {
    }
}
