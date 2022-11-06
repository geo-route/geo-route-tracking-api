using System.Diagnostics;
using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.Api.Middleware
{
	[UsedImplicitly]
	public class RequestLoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<RequestLoggingMiddleware> _logger;

		public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
		{
			this._next = next;
			this._logger = logger;
		}

		[UsedImplicitly]
		public async Task Invoke(HttpContext ctx)
		{
			var id = Guid.NewGuid();
			ctx.Items["RequestId"] = id;

			using var scope = this._logger.BeginScope(new Dictionary<string, object> { ["RequestId"] = id });

			var sw = Stopwatch.StartNew();
			await this._next(ctx).ConfigureAwait(false);
			sw.Stop();

			this._logger.LogInformation(
				"{method} {path} from {ip} resulted in {response} (completion in: {duration}ms)",
				ctx.Request.Method,
				ctx.Request.Path,
				ctx.Request.HttpContext.Connection.RemoteIpAddress,
				ctx.Response.StatusCode,
				sw.ElapsedMilliseconds
			);
		}
	}
}
