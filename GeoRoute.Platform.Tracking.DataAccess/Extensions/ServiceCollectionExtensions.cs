using GeoRoute.Platform.Tracking.DataAccess.Abstract;
using GeoRoute.Platform.Tracking.DataAccess.Repositories;
using GeoRoute.Platform.Tracking.DataMapping;
using Microsoft.Extensions.DependencyInjection;

namespace GeoRoute.Platform.Tracking.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddRepositories(this IServiceCollection collection)
	{
		collection.AddScoped<ITrackingRepository, TrackingRepository>();
	}

	public static void AddDatabaseContexts(this IServiceCollection collection, string connectionString)
	{
		collection.AddScoped(_ => AsyncDynamicDataContext.Create<IMetricContext>(connectionString));
		collection.AddScoped(_ => AsyncDynamicDataContext.Create<ISourceContext>(connectionString));
		collection.AddScoped(_ => AsyncDynamicDataContext.Create<IMeasurementContext>(connectionString));
	}
}
