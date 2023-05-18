using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.Validation;
using VNet.Validation.FluentValidation;

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class CategoryValidator : AbstractValidator<ICategory>, VNet.Validation.IValidator<ICategory>
    {
        public CategoryValidator()
        {
	        RuleSet("Configuration", () =>
	        {
				var errorTag = this.GetType().Name.ToUpper() + "C";
				var i = 0;

				RuleForEach(c => c.Verbs).SetValidator(new VerbValidator(), new string[] { "Configuration" });
				RuleForEach(c => c.Options).SetValidator(new OptionValidator(), new string[] { "Configuration" });

				// name not empty
				RuleFor(verb => verb.Name)
					.NotEmpty()
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot be empty");

				// alternate name not empty
				RuleForEach(verb => verb.AlternateNames)
					.Must(a => !string.IsNullOrEmpty(a))
					.When(v => v.AlternateNames.Any())
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot be empty");

				// name not a reserved verb
				RuleFor(verb => verb.Name)
					.Must(n => !DefaultValues.ReservedVerbs.Contains(n))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot be a reserved word");

				// alternate name not a reserved verb
				RuleForEach(verb => verb.AlternateNames)
					.Must(n => !DefaultValues.ReservedVerbs.Contains(n))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot be a reserved word");

				// names cannot contain special characters
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.SpecialCharacters.Where(s => !string.IsNullOrEmpty(s)).Any(v.Contains))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot contain special characters");

				// alternate names cannot contain special characters
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.SpecialCharacters.Where(s => !string.IsNullOrEmpty(s)).Any(v.Contains))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot contain special characters");

				// names cannot start with category prefixes
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.CategoryPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot start with a category prefix");

				// alternate names cannot start with category prefixes
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.CategoryPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot start with a category prefix");

				// names cannot start with verb prefixes
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.VerbPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot start with a verb prefix");

				// alternate names cannot start with verb prefixes
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.VerbPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot start with a verb prefix");

				// names cannot start with option prefixes
				RuleFor(verb => verb.Name)
					.Must(v => !DefaultValues.OptionPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot start with an option prefix");

				// alternate names cannot start with option prefixes
				RuleForEach(verb => verb.AlternateNames.Select(a => a))
					.Must(v => !DefaultValues.OptionPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(v.StartsWith))
					.Unless(v => v.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Category {PropertyName} cannot start with an option prefix");
			});
        }

        [ExcludeFromCodeCoverage]
		public ValidationState DoValidate(ICategory item, ErrorCategory category)
        {
	        var v = new CategoryValidator();
	        var result = v.Validate(item);

	        return result.ConvertToValidationState(category);
        }

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(ICategory item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
        {
	        if (ruleSets is null) DoValidate(item, defaultCategory);

	        var v = new CategoryValidator();
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