using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using VNet.CommandLine.Attributes;
using VNet.Testing;
using VNet.Utility.Extensions;
// ReSharper disable UnusedType.Global

namespace VNet.CommandLine.Extensions
{
	[ExcludeFromCodeCoverage]
	internal static class IAssemblyExtensions
	{
		internal static IEnumerable<string> AllOptionNames(this IAssembly assembly)
		{
			var optionNames = assembly.PropertiesWithAttribute<OptionAttribute>()
				.SelectMany(c => c.GetCustomAttributes(typeof(OptionAttribute)))
				.Select(a => (OptionAttribute)a).AllOptionNames();

			return optionNames;
		}

		internal static IEnumerable<string> AllVerbNames(this IAssembly assembly)
		{
			var verbNames = assembly.MethodsWithAttribute<VerbAttribute>()
				.SelectMany(c => c.GetCustomAttributes(typeof(VerbAttribute)))
				.Select(a => (VerbAttribute)a).AllVerbNames();

			return verbNames;
		}

		internal static IEnumerable<PropertyInfo> PropertiesWithOptionAttributes(this IAssembly assembly)
		{
			return assembly.PropertiesWithAttribute<OptionAttribute>();
		}

		internal static IEnumerable<MethodInfo> MethodsWithVerbAttributes(this IAssembly assembly)
		{
			var x = assembly.MethodsWithAttribute<VerbAttribute>();
			
			return x;
		}

		internal static IEnumerable<string> AssociatedOptions(this IAssembly assembly)
		{
			var attributes = assembly.MethodsWithAttribute<AssociatedOptionsAttribute>()
				.SelectMany(a => a.GetCustomAttributes(typeof(AssociatedOptionsAttribute), false))
				.Select(a => (AssociatedOptionsAttribute)a);

			var optionNames = attributes.SelectMany(a => a.OptionNames);

			return optionNames;
		}


		internal static IEnumerable<OptionAttribute> OptionAttributes(this IAssembly assembly)
		{
			return assembly.PropertiesWithAttribute<OptionAttribute>()
				.SelectMany(a => a.GetCustomAttributes(typeof(OptionAttribute), false)
				.Select(x => (OptionAttribute)x));
		}

		internal static IEnumerable<AssociatedOptionsAttribute> AssociatedOptionsAttributes(this IAssembly assembly)
		{
			return assembly.PropertiesWithAttribute<AssociatedOptionsAttribute>()
				.SelectMany(a => a.GetCustomAttributes(typeof(AssociatedOptionsAttribute)))
				.Select(x => (AssociatedOptionsAttribute)x);
		}

		internal static IEnumerable<VerbAttribute> VerbAttributes(this IAssembly assembly)
		{
			return assembly.MethodsWithAttribute<VerbAttribute>()
				.SelectMany(a => a.GetCustomAttributes(typeof(VerbAttribute)))
				.Select(x => (VerbAttribute)x);
		}

		internal static IEnumerable<Type> CategoryClasses(this IAssembly assembly)
		{
			return assembly.ClassesWithAttribute<CategoryAttribute>();
		}
	}
}