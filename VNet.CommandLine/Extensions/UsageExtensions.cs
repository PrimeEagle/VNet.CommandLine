using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VNet.CommandLine.Extensions
{
	[ExcludeFromCodeCoverage]
	internal static class UsageExtensions
	{
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

        internal static IEnumerable<string> UsageCategoryNames(this IUsage usage)
        {
            return usage.UsageCategories().Select(c => c.Name);
        }

        internal static IEnumerable<ICategory> UsageCategories(this IUsage usage)
        {
            var result = usage.Categories;

            return result;
        }
	}
}
