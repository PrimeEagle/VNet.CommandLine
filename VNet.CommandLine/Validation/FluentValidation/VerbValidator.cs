using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.CommandLine.Attributes;
using VNet.CommandLine.Extensions;
using VNet.Utility.Extensions;
using VNet.Validation;
using VNet.Validation.FluentValidation;
// ReSharper disable RedundantAssignment

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class VerbValidator : AbstractValidator<IVerb>, VNet.Validation.IValidator<IVerb>
	{
		public VerbValidator()
		{
			RuleSet("Configuration", () =>
			{
				var errorTag = this.GetType().Name.ToUpper() + "C";
				var i = 0;

				//RuleForEach(v => v.Category.Options).SetValidator(new OptionValidator(), new string[] { "Configuration" });

				//AssociatedOptions all refer to real options
				RuleFor(c => c)
					.Must(c => c.AssociatedOptionNames
						.AllExistsIn(c.Category.Configuration.CategoryOptions().Select(o => o.Name)))
					.WithName("Associated Options")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("{PropertyName} must refer to an existing option");

				//// Everything in Verb DependsOn must refer to existing names
				//RuleFor(c => c)
				//	.Must(v => v.DependsOn
				//		.AllExistsIn(v.Category.Configuration.CategoryVerbs().Select(n => n.Name)))
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Verb {PropertyName} refers to a Name that was not found");

				//// Everything in Verb ExclusiveWith must refer to existing names
				//RuleFor(c => c)
				//	.Must(v => v.ExclusiveWith
				//		.AllExistsIn(v.Category.Configuration.CategoryVerbs().Select(n => n.Name)))
				//	.WithName("Exclusive With")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Verb {PropertyName} refers to a Name that was not found");

				// name not empty
				RuleFor(verb => verb.Name)
					.NotEmpty()
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot be empty");

				// alternate name not empty
				RuleForEach(verb => verb.AlternateNames)
					.Must(a => !string.IsNullOrEmpty(a))
					.When(v => v.AlternateNames.Any())
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot be empty");

				// name not a reserved verb
				RuleFor(verb => verb.Name)
					.Must(n => !DefaultValues.ReservedVerbs.Contains(n))
					.Unless(n => n.Attributes.Any(a => a.GetType() == typeof(HelpVerbAttribute) 
					                                   && n.Name == DefaultValues.HelpName))
					.Unless(n => n.Attributes.Any(a => a.GetType() == typeof(VersionVerbAttribute) 
					                                   && n.Name == DefaultValues.VersionName))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot be a reserved word");

				// alternate name not a reserved verb
				RuleForEach(verb => verb.AlternateNames)
					.Must(n => !DefaultValues.ReservedVerbs.Contains(n))
					.Unless(v => v.AlternateNames is null)
					.Unless(n => n.Attributes.Any(a => a.GetType() == typeof(HelpVerbAttribute) 
					                                   && n.AlternateNames.Intersect(DefaultValues.HelpAlternateNames).Any()))
					.Unless(n => n.Attributes.Any(a => a.GetType() == typeof(VersionVerbAttribute) 
					                                   && n.AlternateNames.Intersect(DefaultValues.VersionAlternateNames).Any()))
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot be a reserved word");

				//// verb cannot depend on itself
				//RuleFor(verb => verb)
				//	.Must(v => !v.DependsOn.Contains(v.Name))
				//	.Unless(v => v.DependsOn is null)
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("A verb with {PropertyName} cannot refer to itself");

				//// verb cannot depend on a blank
				//RuleFor(verb => verb)
				//	.Must(v => !v.DependsOn.Any(string.IsNullOrEmpty))
				//	.Unless(v => v.DependsOn is null)
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("A verb {PropertyName} cannot be blank");

				//// verb cannot be exclusive with itself
				//RuleFor(verb => verb)
				//	.Must(v => !v.ExclusiveWith.Contains(v.Name))
				//	.Unless(v => v.ExclusiveWith is null)
				//	.WithName("Exclusive With")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("A verb with {PropertyName} cannot refer to itself");

				//// verb cannot be exclusive with a blank
				//RuleFor(verb => verb)
				//	.Must(v => !v.ExclusiveWith.Any(string.IsNullOrEmpty))
				//	.Unless(v => v.ExclusiveWith is null)
				//	.WithName("Exclusive With")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("A verb {PropertyName} cannot be blank");

				// names cannot contain special characters
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.SpecialCharacters.Where(s => !string.IsNullOrEmpty(s)).Any(v.Contains))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot contain special characters");

				// alternate names cannot contain special characters
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.SpecialCharacters.Where(s => !string.IsNullOrEmpty(s)).Any(v.Contains))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot contain special characters");

				// names cannot start with category prefixes
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.CategoryPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot start with a category prefix");

				// alternate names cannot start with category prefixes
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.CategoryPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot start with a category prefix");

				// names cannot start with verb prefixes
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.VerbPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot start with a verb prefix");

				// alternate names cannot start with verb prefixes
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.VerbPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot start with a verb prefix");

				// names cannot start with option prefixes
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.OptionPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot start with an option prefix");

				// alternate names cannot start with option prefixes
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.OptionPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot start with an option prefix");

				// associated options for each verb must be unique
				RuleFor(v => v)
					.Must(o => o.AssociatedOptionNames
						.Where(s => !string.IsNullOrEmpty(s))
						.GroupBy(g => g)
						.All(x => x.Count() <= 1))
					.Unless(v => v.AssociatedOptionNames is null)
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithName("Associated Options")
					.WithMessage(v => $"{v.Name}: {string.Join(',', v.AssociatedOptionNames)} cannot have duplicates within the same verb");
			});

			RuleSet("Usage", () =>
			{
				var errorTag = this.GetType().Name.ToUpper() + "U";
				var i = 0;

				RuleForEach(v => v.ArgumentOptions).SetValidator(new OptionValidator(), new string[] { "Usage" });

				// no duplicate options without AllowDuplicates
				RuleFor(v => v)
					.Must(c => c.ArgumentOptions
						.Where(o => !o.AllowDuplicates)
						.GroupBy(g => g.Name)
						.All(x => x.Count() <= 1))
					.WithName("Argument Options")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Cannot have duplicate {PropertyName} without AllowDuplicates being true");

				//// dependencies must be present
				//RuleFor(v => v)
				//	.Must(v => v.DependsOn.AllExistsIn(v.Usage.UsageVerbNames()))
				//	.Unless(v => v.DependsOn is null)
				//	.Unless(v => !v.DependsOn.Any())
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Verb dependency is missing");

				//// exclusivities must not be present
				//RuleFor(v => v)
				//	.Must(v => v.ExclusiveWith.NoneExistsIn(v.Usage.UsageVerbNames()))
				//	.Unless(v => v.ExclusiveWith is null)
				//	.Unless(v => !v.ExclusiveWith.Any())
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Verb exclusivity is present");
			});
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IVerb item, ErrorCategory category)
		{
			var v = new VerbValidator();
			var result = v.Validate(item);

			return result.ConvertToValidationState(category);
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IVerb item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
		{
			if (ruleSets is null) DoValidate(item, defaultCategory);

			var v = new VerbValidator();
			var result = new ValidationState();

			// ReSharper disable once PossibleNullReferenceException
			foreach (var rs in ruleSets)
			{
				var rsResult = v.Validate(item, p => { p.IncludeRuleSets(rs); });

				var errorCategory = defaultCategory;
				if (Enum.TryParse(typeof(ErrorCategory), rs, true, out var cat))
				{
					if (cat != null) errorCategory = (ErrorCategory)cat;
				}

				result.Append(rsResult.ConvertToValidationState(errorCategory));
			}

			return result;
		}
	}
}