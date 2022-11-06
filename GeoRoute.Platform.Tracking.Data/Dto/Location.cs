namespace GeoRoute.Platform.Tracking.Data.Dto;

public record Location
{
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
