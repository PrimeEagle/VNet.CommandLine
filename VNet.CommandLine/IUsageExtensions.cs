using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using VNet.CommandLine.Attributes;
using VNet.Testing;
using VNet.Utility.Extensions;

namespace VNet.CommandLine
{
    [ExcludeFromCodeCoverage]
    internal static class IUsageExtensions
    {
        internal static IEnumerable<string> AllOptionNames(this IAssembly assembly)
        {
            var optionNames = assembly.PropertiesWithAttribute<OptionAttribute>()
                .SelectMany(c => c.GetCustomAttributes(typeof(OptionAttribute)))
                .Select(a => (OptionAttribute)a).AllOptionNames();

            return optionNames;
        }

        internal static IEnumerable<IOption> CategoryOptions(this IConfiguration configuration)
        {
	        var result = configuration.Categories.SelectMany(c => c.Options);

	        return result;
        }

        internal static IEnumerable<IVerb> CategoryVerbs(this IConfiguration configuration)
        {
	        var result = configuration.Categories.SelectMany(c => c.Verbs);

	        return result;
        }

        internal static IEnumerable<string> AssociatedOptions(this IConfiguration configuration)
        {
	        var result = configuration.Categories.SelectMany(c => c.Verbs.SelectMany(v => v.AssociatedOptionNames));

	        return result;
        }

        internal static IEnumerable<IOption> ArgumentOptions(this IConfiguration configuration)
        {
	        var result = configuration.Categories.SelectMany(c => c.Verbs.SelectMany(v => v.ArgumentOptions));

	        return result;
        }

        internal static IEnumerable<string> CategoryAllNames(this IConfiguration configuration)
        {
	        var result = configuration.CategoryNames().Concat(configuration.CategoryAlternateNames());

	        return result;
        }

        internal static IEnumerable<string> CategoryNames(this IConfiguration configuration)
        {
	        return configuration.Categories.Select(c => c.Name);
        }

        internal static IEnumerable<string> CategoryAlternateNames(this IConfiguration configuration)
        {
	        return configuration.Categories.SelectMany(c => c.AlternateNames);
        }

        internal static IEnumerable<string> CategoryVerbNames(this IConfiguration configuration)
        {
	        return configuration.CategoryVerbs().Select(c => c.Name);
        }

        internal static IEnumerable<string> CategoryVerbAlternateNames(this IConfiguration configuration)
        {
	        return configuration.CategoryVerbs().SelectMany(c => c.AlternateNames);
        }

        internal static IEnumerable<string> CategoryVerbAllNames(this IConfiguration configuration)
        {
	        var result = configuration.CategoryVerbNames().Concat(configuration.CategoryVerbAlternateNames());

	        return result;
        }

        internal static IEnumerable<string> CategoryOptionNames(this IConfiguration configuration)
        {
	        return configuration.CategoryOptions().Select(c => c.Name);
        }

        internal static IEnumerable<string> CategoryOptionAlternateNames(this IConfiguration configuration)
        {
	        return configuration.CategoryOptions().SelectMany(c => c.AlternateNames);
        }

        internal static IEnumerable<string> CategoryOptionAllNames(this IConfiguration configuration)
        {
	        var result = configuration.CategoryOptionNames().Concat(configuration.CategoryOptionAlternateNames());

	        return result;
        }

        internal static IEnumerable<string> AllOptionNames(this IEnumerable<IOptionAttribute> options)
        {
            var optionAttributes = options.ToList();
            var names = optionAttributes.Select(n => n.Name);
            var alternateNames = optionAttributes.SelectMany(n => n.AlternateNames);

            var result = names.Concat(alternateNames);

            return result;
        }


        internal static IEnumerable<string> UsageOptionNames(this IUsage usage)
        {
	        return usage.UsageOptions().Select(c => c.Name);
        }

        internal static IEnumerable<IOption> UsageOptions(this IUsage usage)
        {
	        var result = usage.Verbs.SelectMany(v => v.ArgumentOptions);

	        return result;
        }

        internal static IEnumerable<string> UsageVerbNames(this IUsage usage)
        {
	        return usage.UsageVerbs().Select(c => c.Name);
        }

        internal static IEnumerable<IVerb> UsageVerbs(this IUsage usage)
        {
	        var result = usage.Verbs;

	        return result;
        }

        internal static IEnumerable<string> AllOptionNames(this IEnumerable<IOption> optionList)
        {
            var enumerable = optionList.ToList();
            var result = enumerable.OptionNames().Concat(enumerable.OptionAlternateNames());

            return result;
        }

        internal static IEnumerable<string> AllVerbNames(this IAssembly assembly)
        {
            var verbNames = assembly.MethodsWithAttribute<VerbAttribute>()
                .SelectMany(c => c.GetCustomAttributes(typeof(VerbAttribute)))
                .Select(a => (VerbAttribute)a).AllVerbNames();

            return verbNames;
        }

        internal static IEnumerable<string> AllVerbNames(this IEnumerable<IVerbAttribute> verbs)
        {
            var verbAttributes = verbs.ToList();
            var names = verbAttributes.Select(n => n.Name);
            var alternateNames = verbAttributes.SelectMany(n => n.AlternateNames);

            var result = names.Concat(alternateNames);

            return result;
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

        internal static IOption FindOption(string argument, IEnumerable<IOption> configuredOptions)
        {
            IOption resultOption = null;
            var term = argument;

            var idx = -1;
            var found = false;

            foreach (var c in term)
            {
                idx++;
                if (!DefaultValues.OptionValueSeparators.Contains(c.ToString())) continue;

                found = true;
                break;
            }

            if (found && idx >= 0)
            {
                term = argument.Before(argument[idx].ToString());
            }

            foreach (var prefix in DefaultValues.OptionPrefixes)
            {
	            // ReSharper disable once PossibleMultipleEnumeration
	            foreach (var option in configuredOptions.ToList())
                {
                    var names = new List<string> { option.Name };
                    names.AddRange(option.AlternateNames.Select(r => r).Distinct().ToList());

                    foreach (var comboName in names.Select(n => $"{prefix}{n}"))
                    {
                        if (comboName == term)
                        {
                            resultOption = option;
                        }

                        if (resultOption != null) break;
                    }

                    if (resultOption != null) break;
                }

                if (resultOption != null) break;
            }

            return resultOption;
        }

        internal static IVerb FindVerb(string argument, IConfiguration configuration)
        {
            IVerb resultVerb = null;

            foreach (var prefix in DefaultValues.VerbPrefixes)
            {
	            // ReSharper disable once PossibleMultipleEnumeration
	            var configuredVerbs = configuration.Categories.SelectMany(c => c.Verbs).ToList();

                foreach (var verb in configuredVerbs)
                {
                    var names = new List<string> { verb.Name };
                    names.AddRange(verb.AlternateNames.Select(r => r).Distinct().ToList());

                    foreach (var comboName in names.Select(n => $"{prefix}{n}"))
                    {
                        if (comboName == argument)
                        {
                            resultVerb = verb;
                        }

                        if (resultVerb != null) break;
                    }

                    if (resultVerb != null) break;
                }

                if (resultVerb != null) break;
            }

            return resultVerb;
        }

        internal static IEnumerable<OptionAttribute> OptionAttributes(this Type classType)
        {
            return classType.GetCustomAttributes<OptionAttribute>();
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

        internal static IEnumerable<string> OptionAlternateNames(this IEnumerable<IOption> optionList)
        {
            var optAlternateNames = optionList.SelectMany(c => c.AlternateNames);

            return optAlternateNames;
        }

        internal static IEnumerable<string> OptionNames(this IEnumerable<IOptionAttribute> options)
        {
            var names = options.Select(n => n.Name);

            return names;
        }

        //    return verbNames.Concat(optionNames);
        //}
        internal static IEnumerable<string> OptionNames(this IEnumerable<IOption> optionList)
        {
            var optNames = optionList.Select(c => c.Name);

            return optNames;
        }

        internal static IEnumerable<IVerb> OrderForDisplay(this IEnumerable<IVerb> commandList)
        {
            var sortedList = commandList.OrderBy(a => a.Name);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var s in sortedList)
            {
                var tempOptions = s.Category.Options.OrderBy(a => a.Name).ToList();

                s.Category.Options.Clear();
                s.Category.Options.AddRange(tempOptions);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return sortedList;
        }

        internal static IEnumerable<IVerb> OrderForExecution(this IEnumerable<IVerb> commandList)
        {
            var commandVerbs = commandList.ToList();

            var defaultVerb = commandVerbs.FirstOrDefault(c => c.Attributes.Any<IAttribute>(a => a.GetType() == typeof(DefaultVerbAttribute)));
            var unsorted = commandVerbs.Where(c => !c.ExecutionOrder.HasValue);
            var sortedList = commandVerbs.Where(c => c.ExecutionOrder.HasValue).OrderBy(a => a.ExecutionOrder);

            var result = new List<IVerb>();

            if (defaultVerb is not null)
            {
	            result.Add(defaultVerb);
            }

            result.AddRange(sortedList);
            result.AddRange(unsorted);

            return result;
        }

        internal static IEnumerable<VerbAttribute> VerbAttributes(this IAssembly assembly)
        {
            return assembly.MethodsWithAttribute<VerbAttribute>()
                .SelectMany(a => a.GetCustomAttributes(typeof(VerbAttribute)))
                .Select(x => (VerbAttribute)x);
        }

        internal static IEnumerable<string> VerbNames(this IEnumerable<IVerbAttribute> verbs)
        {
            var names = verbs.Select(n => n.Name);

            return names;
        }
    }
}
