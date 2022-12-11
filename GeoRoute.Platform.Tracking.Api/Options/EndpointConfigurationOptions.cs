namespace CM.Messaging.Smsl.CarrierMTRouter.Options;

public class EndpointConfigurationOptions
{
	public string? Host { get; set; }
	public int? Port { get; set; }
	public string? Scheme { get; set; }
	public int Version { get; set; }
	public string? StoreName { get; set; }
	public string? StoreLocation { get; set; }
	public string? FilePath { get; set; }
	public string? Password { get; set; }
}
