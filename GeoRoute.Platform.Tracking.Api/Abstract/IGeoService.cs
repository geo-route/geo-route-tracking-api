using GeoRoute.Platform.Tracking.Data.Dto;

namespace GeoRoute.Platform.Tracking.Api.Abstract;

public interface IGeoService
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "AV1704:Identifier contains one or more digits in its name", Justification = "Better than not using numbers.")]
    double GetDistance(Location p1, Location p2);
}
