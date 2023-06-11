using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable MemberCanBePrivate.Global

namespace VNet.CommandLine
{
	[ExcludeFromCodeCoverage]
	public static class DefaultValues
	{
		public static string HelpName { get; set; }

		public static IEnumerable<string> HelpAlternateNames { get; set; }
		public static string HelpHelpText { get; set; }
		public static string VersionName { get; set; }
		public static IEnumerable<string> VersionAlternateNames { get; set; }
		public static string VersionHelpText { get; set; }
		public static IEnumerable<string> ReservedVerbs
		{
			get
			{
				var result = new List<string>
				{
					DefaultValues.HelpName,
					DefaultValues.VersionName
				};
				result.AddRange(DefaultValues.HelpAlternateNames);
				result.AddRange(DefaultValues.VersionAlternateNames);

				return result;
			}
		}

		public static IList<string> CategoryPrefixes { get; set; }
		public static IList<string> VerbPrefixes { get; set; }
		public static IList<string> OptionPrefixes { get; set; }
		public static IList<string> OptionValueSeparators { get; set; }
		public static IList<string> OptionValueArrayStartDelimiters { get; set; }
		public static IList<string> OptionValueArrayEndDelimiters { get; set; }
		public static IList<string> OptionValueArrayDelimiters { get; set; }
		public static IList<Type> AllowedDataTypes { get; set; }
		public static IEnumerable<string> SpecialCharacters
		{
			get
			{
				var specials = DefaultValues.CategoryPrefixes.SelectMany(c => c.Where(n => !char.IsLetterOrDigit(n)))
					.Concat(DefaultValues.VerbPrefixes.SelectMany(v => v.Where(n => !char.IsLetterOrDigit(n))))
					.Concat(DefaultValues.OptionPrefixes.SelectMany(o => o.Where(n => !char.IsLetterOrDigit(n))))
					.Concat(DefaultValues.OptionValueArrayStartDelimiters.SelectMany(o => o.Where(n => !char.IsLetterOrDigit(n))))
					.Concat(DefaultValues.OptionValueArrayEndDelimiters.SelectMany(o => o.Where(n => !char.IsLetterOrDigit(n))))
					.Concat(DefaultValues.OptionValueArrayDelimiters.SelectMany(o => o.Where(n => !char.IsLetterOrDigit(n))))
					.Concat(DefaultValues.OptionValueSeparators.SelectMany(o => o.Where(n => !char.IsLetterOrDigit(n))));

				return specials.Select(c => c.ToString()).Where(s => s.Length > 0).Distinct().ToList();
			}
		}
	}
}