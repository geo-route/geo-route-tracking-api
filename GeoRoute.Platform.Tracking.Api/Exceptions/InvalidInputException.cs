using System.Net;

namespace GeoRoute.Platform.Tracking.Api.Exceptions;

public class InvalidInputException : HttpException
{
	public InvalidInputException(string message, HttpStatusCode statusCode) : base(message, statusCode)
	{
	}
}
