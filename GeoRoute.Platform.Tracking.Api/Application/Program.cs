using System.Reflection;
using Serilog;

namespace GeoRoute.Platform.Tracking.Api.Application;

public static class Program
{
    public static void Main(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var config = BuildConfiguration(env);
        Log.Logger = LoggingBuilder.BuildLogger(config, env);

        Log.Logger.Information("Starting {app} {version}",
            Assembly.GetExecutingAssembly().GetName().Name,
            Assembly.GetExecutingAssembly().GetName().Version);
        var builder = CreateHostBuilder(args, config);
        var host = builder.Build();

        host.Run();
    }

    private static IConfigurationRoot BuildConfiguration(string env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    private static bool IsDevelopment()
    {
	    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
	    return env.ToUpperInvariant().Equals("DEVELOPMENT");
    }

    private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration conf)
    {
        return Host.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureLogging((context, builder) => {
                builder.ClearProviders();
                builder.AddSerilog();

                if(context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsStaging()) {
                    builder.AddDebug();
                }
            })
            .ConfigureAppConfiguration((_, config) => {
                config.AddConfiguration(conf);
            })


            .ConfigureWebHostDefaults(webBuilder => { webBuilder.ConfigureKestrel(opts => {
	            if(!IsDevelopment()) {
		            return;
	            }

	            var portText = webBuilder.GetSetting("ApiSettings:Port");
	            var port = int.Parse(portText!);
                opts.ListenAnyIP(port);
            }).UseStartup<Startup>(); });
    }
}
