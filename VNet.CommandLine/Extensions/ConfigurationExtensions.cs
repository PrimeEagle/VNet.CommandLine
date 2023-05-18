using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VNet.CommandLine.Extensions
{
	[ExcludeFromCodeCoverage]
    internal static class ConfigurationExtensions
    {
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
	        return configuration.CategoryNames().Concat(configuration.CategoryAlternateNames());
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
	        return configuration.CategoryVerbNames().Concat(configuration.CategoryVerbAlternateNames());
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
	        return configuration.CategoryOptionNames().Concat(configuration.CategoryOptionAlternateNames());
        }
    }
}
