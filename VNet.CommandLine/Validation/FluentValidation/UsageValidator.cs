using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.Validation;
using VNet.Validation.FluentValidation;

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class UsageValidator : AbstractValidator<IUsage>, VNet.Validation.IValidator<IUsage>
	{
		public UsageValidator()
		{
			RuleSet("Usage", () =>
			{
				var errorTag = this.GetType().Name.ToUpper() + "U";
				var i = 0;

				RuleForEach(u => u.Verbs).SetValidator(new VerbValidator(), new string[] { "Usage" });

				// no duplicate categories
				RuleFor(u => u)
					.Must(u => u.Categories
						.Select(v => v.Name)
						.GroupBy(g => g)
						.All(x => x.Count() <= 1))
					.WithName("Categories")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Cannot have duplicate categories");

				// no duplicate verbs
				RuleFor(u => u)
					.Must(u => u.Verbs
						.Select(v => v.Name)
						.GroupBy(g => g)
						.All(x => x.Count() <= 1))
					.WithName("Verbs")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Cannot have duplicate verbs");
			});
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IUsage item, ErrorCategory category)
		{
			var v = new UsageValidator();
			var result = v.Validate(item);

			return result.ConvertToValidationState(category);
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IUsage item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
		{
			if (ruleSets is null) DoValidate(item, defaultCategory);

			var v = new UsageValidator();
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