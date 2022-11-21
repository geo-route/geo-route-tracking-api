namespace GeoRoute.Platform.Tracking.Data.Egress;

public record ProximityResult
{
	public string Result { get; set; }
    public bool WithinProximity { get; set; }
}
