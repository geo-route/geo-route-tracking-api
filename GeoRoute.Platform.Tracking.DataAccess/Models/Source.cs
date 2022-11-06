using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.DataAccess.Models;

[PublicAPI]
public class Source
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public DateTime Timestamp { get; init; }
}
