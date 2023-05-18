using System;
using System.Collections.Generic;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine
{
	public interface IBaseCommand
    {
	    string Name { get; set; }
	    List<string> AlternateNames { get; set; }
	    List<string> AllNames { get; }
		bool Required { get; set; }
		bool Allowed { get; set; }
		Help Help { get; set; }
        List<ICondition> ConfigurationConditions { get; }
        List<ICondition> UsageConditions { get; }
        List<ICondition> Conditions { get; set; }
		IUsage Usage { get; set; }
		IConfiguration Configuration { get; set; }
		Type SourceBaseType { get; set; }
		List<IAttribute> Attributes { get; set; }


		BaseCommand Clone();
    }
}
