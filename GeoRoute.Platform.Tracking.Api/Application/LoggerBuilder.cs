using Serilog;

namespace GeoRoute.Platform.Tracking.Api.Application
{
    internal static class LoggingBuilder
    {
        internal static Serilog.Core.Logger BuildLogger(IConfigurationRoot config, string env)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                .Enrich.WithProperty("Environment", env)
                .CreateLogger();
        }
    }
}
