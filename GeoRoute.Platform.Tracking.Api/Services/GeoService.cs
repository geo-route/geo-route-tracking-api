using GeoRoute.Platform.Tracking.Api.Abstract;
using GeoRoute.Platform.Tracking.Data.Dto;

namespace GeoRoute.Platform.Tracking.Api.Services;

public class GeoService : IGeoService
{
	private const double EarthRadius = 6371000;

    private static double DegToRad(double deg)
	{
		return deg * (Math.PI / 180);
	}

	public double GetDistance(Location p1, Location p2)
	{
		var dLat = DegToRad(Convert.ToDouble(p2.Latitude - p1.Latitude));
		var dLng = DegToRad(Convert.ToDouble(p2.Longitude - p1.Longitude));
		var x =
			Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
			Math.Cos(DegToRad(Convert.ToDouble(p1.Latitude))) * Math.Cos(DegToRad(Convert.ToDouble(p2.Latitude))) *
			Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
		var arc = 2 * Math.Atan2(Math.Sqrt(x), Math.Sqrt(1 - x));

		return EarthRadius * arc;
	}
}
