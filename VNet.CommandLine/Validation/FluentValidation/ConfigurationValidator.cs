using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using VNet.CommandLine.Attributes;
using VNet.Utility.Extensions;
using VNet.Validation;
using VNet.Validation.FluentValidation;

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class ConfigurationValidator : AbstractValidator<IConfiguration>, VNet.Validation.IValidator<IConfiguration>
	{
		public ConfigurationValidator()
		{
			RuleSet("Configuration", () =>
			{
				var errorTag = this.GetType().Name.ToUpper() + "C";
				var i = 0;

				RuleForEach(co => co.Categories).SetValidator(new CategoryValidator(), new string[] { "Configuration" });

				// configuration must have at least one verb
				RuleFor(c => c)
					.Must(v => v.CategoryVerbs().Any())
					.WithName("Category Verbs")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Configuration must have at least one verb");

				// [Help] is used no more than one time
				RuleFor(c => c)
					.Must(d => d.CategoryVerbs()
						.Count(x => x.Attributes.Any(a => a.GetType() == typeof(HelpVerbAttribute))) <= 1)
					.WithName("HelpAttribute")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("The {PropertyName} must be used no more than one time");

				// [Version] is used no more than one time
				RuleFor(c => c)
					.Must(d => d.CategoryVerbs()
						.Count(v => v.Attributes.Any(a => a.GetType() == typeof(VersionVerbAttribute))) <= 1)
					.WithName("VersionAttribute")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("The {PropertyName} must be used no more than one time");

				// [DefaultVerb] is used no more than one time
				RuleFor(c => c)
					.Must(d => d.CategoryVerbs()
						.Count(v => v.Attributes.Any(a => a.GetType() == typeof(DefaultVerbAttribute))) <= 1)
					//.Must(d => d.Count(v => v.Attributes.Any(a => a.GetType() == typeof(DefaultVerbAttribute))) <= 1)
					.WithName("DefaultVerbAttribute")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("The {PropertyName} must be used no more than one time");

				// No duplicates across category, verb, and option names or alternate names
				RuleFor(c => c)
					.Must(v => v.CategoryAllNames().Where(n => !string.IsNullOrEmpty(n))
						.Concat(v.CategoryVerbAllNames().Where(n => !string.IsNullOrEmpty(n)))
						.Concat(v.CategoryOptionAllNames().Where(n => !string.IsNullOrEmpty(n)))
						.Select(s => s)
						.GroupBy(g => g)
						.All(x => x.Count<string>() <= 1))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category, verb, and option {PropertyName} cannot have duplicates")
					.DependentRules(() =>
					{
						// No duplicate category names or alternate names
						RuleFor(c => c)
							.Must(v => v.CategoryAllNames().Where(n => !string.IsNullOrEmpty(n))
								.Select(s => s)
								.GroupBy(g => g)
								.All(x => x.Count<string>() <= 1))
							.WithName("Name")
							.WithErrorCode($"{errorTag}{(i++):###}")
							.WithMessage("Category {PropertyName} cannot have duplicates");

						// No duplicate verb names or alternate names
						RuleFor(c => c)
							.Must(v => v.CategoryVerbAllNames().Where(n => !string.IsNullOrEmpty(n))
								.Select(s => s)
								.GroupBy(g => g)
								.All(x => x.Count<string>() <= 1))
							.WithName("Name")
							.WithErrorCode($"{errorTag}{(i++):###}")
							.WithMessage("Verb {PropertyName} cannot have duplicates");

						// No duplicate option names or alternate names
						RuleFor(c => c)
							.Must(o => o.CategoryOptionAllNames().Where(n => !string.IsNullOrEmpty(n))
								.Select(s => s)
								.GroupBy(g => g)
								.All(x => x.Count<string>() <= 1))
							.WithName("Name")
							.WithErrorCode($"{errorTag}{(i++):###}")
							.WithMessage("Option {PropertyName} cannot have duplicates");
					});

				// all options must be associated to a verb
				RuleFor(c => c)
					.Must(c => c.AssociatedOptions()
						.AllExistsIn(c.CategoryOptions().Select(o => o.Name)))
					.WithName("Category Options")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("All options must be associated to at least one verb");

				//// each category display order can only be used once
				//RuleFor(c => c)
				//	.Must(c => c.Categories
				//		.Where(e => e.DisplayOrder > 0)
				//		.Select(e => e.DisplayOrder)
				//		.GroupBy(g => g)
				//		.All(x => x.Count() <= 1))
				//	.WithName("Display Order")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Category {PropertyName} cannot be duplicated");

				// each verb execution order can only be used once
				RuleFor(c => c)
					.Must(v => v.CategoryVerbs()
						.Where(e => e.ExecutionOrder > 0)
						.Select(e => e.ExecutionOrder)
						.GroupBy(g => g)
						.All(x => x.Count() <= 1))
					.WithName("Execution Order")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Verb {PropertyName} cannot be duplicated");
			});
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IConfiguration item, ErrorCategory category)
		{
			var v = new ConfigurationValidator();
			var result = v.Validate(item);

			return result.ConvertToValidationState(category);
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IConfiguration item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
		{
			if (ruleSets is null) DoValidate(item, defaultCategory);

			var v = new ConfigurationValidator();
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