using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.Validation;
using VNet.Validation.FluentValidation;

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class ArgumentStartValidator : AbstractValidator<IArgumentStart>, VNet.Validation.IValidator<IArgumentStart>
    {
        public ArgumentStartValidator()
        {
	        RuleSet("Usage", () =>
	        {
		        var errorTag = this.GetType().Name.ToUpper() + "C";
		        var i = 0;

		        // value cannot contain more than one separator
		        RuleFor(a => a)
			        .Must(a => a.DefaultVerb is not null)
			        .When(a => a.Option is not null)
			        .When(a => !a.Verbs.Any())
			        .WithErrorCode($"{errorTag}{(i++):###}")
			        .WithMessage("Command line arguments must start with a verb, unless a default verb is defined.");
	        });
        }

        [ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IArgumentStart item, ErrorCategory category)
        {
	        var v = new ArgumentStartValidator();
	        var result = v.Validate(item);

	        return result.ConvertToValidationState(category);
        }

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IArgumentStart item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
        {
	        if (ruleSets is null) DoValidate(item, defaultCategory);

	        var v = new ArgumentStartValidator();
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