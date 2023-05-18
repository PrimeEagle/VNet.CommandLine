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
    internal static class TypeExtensions
    {
	    internal static IEnumerable<IConditionAttribute> CategoryConditionAttributes(this Type type)
	    {
		    var result = new List<IConditionAttribute>();
			result.AddRange(type.GetCustomAttributes<ConditionAttribute>().ToList());

			return result;
	    }

	    internal static IHelpAttribute CategoryHelpAttribute(this Type type)
	    {
		    return (IHelpAttribute)type.GetCustomAttribute<HelpAttribute>();
	    }

		internal static IEnumerable<MethodInfo> VerbMethods(this Type type)
	    {
		    var result = type.MethodsWithAttribute<VerbAttribute>().ToList();
		    result.AddRange(type.MethodsWithAttribute<HelpVerbAttribute>().ToList());
		    result.AddRange(type.MethodsWithAttribute<VersionVerbAttribute>().ToList());

		    return result;
	    }

	    internal static IEnumerable<PropertyInfo> OptionProperties(this Type type)
	    {
		    return type.PropertiesWithAttribute<OptionAttribute>();
	    }

        internal static IEnumerable<OptionAttribute> OptionAttributes(this Type type)
        {
            return type.GetCustomAttributes<OptionAttribute>();
        }
    }
}
