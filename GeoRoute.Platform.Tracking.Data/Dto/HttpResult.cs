namespace GeoRoute.Platform.Tracking.Data.Dto;

public class HttpResult<TValue>
{
    public Guid Id { get; set; }
    public TValue? Data { get; set; }
    public ICollection<string>? Errors { get; set; }

    public HttpResult()
    {
        this.Id = Guid.NewGuid();
    }
}
