using System;
using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.DataMapping.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class ProcedureNameAttribute : Attribute
{
	public string Name { get; }

	public ProcedureNameAttribute(string name)
	{
		this.Name = name;
	}
}
