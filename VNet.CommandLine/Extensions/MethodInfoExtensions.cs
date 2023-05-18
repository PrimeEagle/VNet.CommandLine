using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using VNet.CommandLine.Attributes;
using VNet.Utility.Extensions;

namespace VNet.CommandLine.Extensions
{
	[ExcludeFromCodeCoverage]
    internal static class MethodInfoExtensions
	{
	    internal static IEnumerable<IVerbAttribute> VerbAttributes(this MethodInfo mi)
	    {
			var result = new List<IVerbAttribute>();
			result.AddRange(mi.Attributes<VerbAttribute>().ToList());
			result.AddRange(mi.Attributes<HelpVerbAttribute>());
			result.AddRange(mi.Attributes<VersionVerbAttribute>());

			return result;
	    }

	    internal static IEnumerable<IConditionAttribute> VerbConditionAttributes(this MethodInfo mi)
	    {
		    var result = new List<IConditionAttribute>();

		    result.AddRange(mi.GetCustomAttributes<ConditionAttribute>().ToList());

		    return result;
	    }

	    internal static IHelpAttribute VerbHelpAttribute(this MethodInfo mi)
	    {
		    return (IHelpAttribute)mi.GetCustomAttribute<HelpAttribute>();
	    }

	    internal static DefaultVerbAttribute VerbDefaultVerbAttribute(this MethodInfo mi)
	    {
		    return (DefaultVerbAttribute)mi.GetCustomAttribute<DefaultVerbAttribute>();
	    }

		internal static IAssociatedOptionsAttribute VerbAssociatedOptionsAttribute(this MethodInfo mi)
	    {
		    return (IAssociatedOptionsAttribute)mi.GetCustomAttribute<AssociatedOptionsAttribute>();
	    }
	}
}
