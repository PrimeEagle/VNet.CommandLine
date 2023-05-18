using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VNet.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    [ExcludeFromCodeCoverage]
    public class VersionVerbAttribute : Attribute, IVerbAttribute
    {
	    public string Name { get; set; }
	    public string[] AlternateNames { get; set; }
	    public string[] AllNames
	    {
		    get
		    {
			    var result = new List<string> { this.Name.ToLower() };
			    result.AddRange(this.AlternateNames.Select(n => n.ToLower()));

			    return result.ToArray();
		    }
	    }
	    public bool Required { get; set; }
	    public bool Allowed { get; set; }
	    public int ExecutionOrder { get; set; }

	    public VersionVerbAttribute()
	    {
		    this.Name = DefaultValues.VersionName;
		    this.AlternateNames = Array.Empty<string>();
			this.Allowed = true;
	    }
	}
}