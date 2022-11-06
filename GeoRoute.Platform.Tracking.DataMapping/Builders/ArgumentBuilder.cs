using Dapper;

namespace GeoRoute.Platform.Tracking.DataMapping.Builders;

public class ArgumentBuilder
{
    private readonly DynamicParameters _parameters;

    public ArgumentBuilder()
    {
        this._parameters = new DynamicParameters();
    }

    public ArgumentBuilder WithArgument(string name, object @object)
    {
        this._parameters.Add($"@{name}", @object);
        return this;
    }

    public DynamicParameters Build()
    {
        return this._parameters;
    }
}
