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
    internal static class PropertyInfoExtensions
	{
		internal static IEnumerable<IOptionAttribute> OptionAttributes(this PropertyInfo pi)
		{
			var result = new List<IOptionAttribute>();
			result.AddRange(pi.Attributes<OptionAttribute>().ToList());

			return result;
		}

		internal static IEnumerable<IConditionAttribute> OptionConditionAttributes(this PropertyInfo pi)
		{
			var result = new List<IConditionAttribute>();

			result.AddRange(pi.GetCustomAttributes<ConditionAttribute>().ToList());

			return result;
		}

		internal static IHelpAttribute OptionHelpAttribute(this PropertyInfo pi)
		{
			return (IHelpAttribute)pi.GetCustomAttribute<HelpAttribute>();
		}
	}
}
