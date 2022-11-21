using System.Text.Json;
using GeoRoute.Platform.Tracking.Api.Abstract;
using GeoRoute.Platform.Tracking.Api.Middleware;
using GeoRoute.Platform.Tracking.Api.Services;
using GeoRoute.Platform.Tracking.DataAccess.Extensions;
using NSwag;

namespace GeoRoute.Platform.Tracking.Api.Application;

public class Startup
{
    private readonly IConfiguration _config;

    public Startup(IConfiguration configuration)
    {
        this._config = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddDatabaseContexts(this._config.GetConnectionString("GeoRoute"));
        services.AddRepositories();
        services.AddSingleton<IGeoService, GeoService>();

        services.AddRouting(options => {
            options.LowercaseQueryStrings = true;
            options.LowercaseUrls = true;
        });

        ConfigureSwaggerDocument(services);

        services.AddControllers().AddJsonOptions(options => {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ConfigureSettings(app, env);
        ConfigureCors(app);

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ErrorResponseMiddleware>();

        this.ConfigureOpenApi(app);

        ConfigureSwaggerUi(app);
        ConfigureEndpoints(app);
    }

    private static void ConfigureCors(IApplicationBuilder app)
    {
        app.UseCors(corsPolicyBuilder => {
            corsPolicyBuilder.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    }

    private static void ConfigureSettings(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseForwardedHeaders();
        app.UseRouting();

        if(env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }
    }

    private void ConfigureOpenApi(IApplicationBuilder app)
    {
        var apiSection = this._config.GetSection("ApiSettings");
        var host = apiSection?.GetValue<string>("Host") ?? "localhost";
        var port = apiSection?.GetValue<int>("Port") ?? 80;
        var schema = GetSchema($"{host}:{port}");

        app.UseOpenApi(openApi => {
            openApi.Path = "/geo-route/tracking/swagger.json";
            openApi.PostProcess = (settings, _) => {
                settings.Host = $"{host}:{port}";
                settings.Schemes.Clear();
                settings.Schemes.Add(schema);
            };
        });
    }

    private static OpenApiSchema GetSchema(string host)
    {
        var schema = OpenApiSchema.Http;

        if(host.Contains("https")) {
            schema = OpenApiSchema.Https;
        }

        return schema;
    }

    private static void ConfigureEndpoints(IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }

    private static void ConfigureSwaggerDocument(IServiceCollection services)
    {
        services.AddSwaggerDocument(
            document => {
                document.GenerateEnumMappingDescription = true;
                document.GenerateExamples = true;
                document.Title = "GeoRoute Tracking API";
                document.Version = typeof(Startup).Assembly.GetName().Version?.ToString();
            });
    }

    private static void ConfigureSwaggerUi(IApplicationBuilder app)
    {
        app.UseSwaggerUi3(api => {
            api.Path = "/geo-route/tracking/swagger";
            api.DocumentPath = "/geo-route/tracking/swagger.json";
        });
    }
}
