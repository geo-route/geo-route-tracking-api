using System;
using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.DataMapping.Attributes;

[AttributeUsage(AttributeTargets.Class)]
[PublicAPI]
public sealed class TableValuedParameterAttribute : Attribute
{
    public string Name { get; }

    public TableValuedParameterAttribute(string name)
    {
        this.Name = name;
    }
}
