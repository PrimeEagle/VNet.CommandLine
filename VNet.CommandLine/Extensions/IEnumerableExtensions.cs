using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.CommandLine.Attributes;
using VNet.Utility.Extensions;

// ReSharper disable InconsistentNaming

namespace VNet.CommandLine.Extensions
{
	[ExcludeFromCodeCoverage]
	internal static class IEnumerableExtensions
	{
		internal static IEnumerable<string> AllOptionNames(this IEnumerable<IOptionAttribute> options)
		{
			var optionAttributes = options.ToList();
			var names = optionAttributes.Select(n => n.Name);
			var alternateNames = optionAttributes.SelectMany(n => n.AlternateNames);

			var result = names.Concat(alternateNames);

			return result;
		}


		internal static IEnumerable<string> AllOptionNames(this IEnumerable<IOption> optionList)
		{
			var enumerable = optionList.ToList();
			var result = enumerable.OptionNames().Concat(enumerable.OptionAlternateNames());

			return result;
		}


		internal static IEnumerable<string> AllVerbNames(this IEnumerable<IVerbAttribute> verbs)
		{
			var verbAttributes = verbs.ToList();
			var names = verbAttributes.Select(n => n.Name);
			var alternateNames = verbAttributes.SelectMany(n => n.AlternateNames);

			var result = names.Concat(alternateNames);

			return result;
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


		internal static IEnumerable<string> VerbNames(this IEnumerable<IVerbAttribute> verbs)
		{
			var names = verbs.Select(n => n.Name);

			return names;
		}
	}
}
