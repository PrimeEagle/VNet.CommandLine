using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using VNet.Validation;
using VNet.Validation.FluentValidation;

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class ArgumentArrayValueValidator : AbstractValidator<IArgumentArrayValue>, VNet.Validation.IValidator<IArgumentArrayValue>
    {
        public ArgumentArrayValueValidator()
        {
            // array must start with a valid delimiter
            RuleFor(a => a.ArrayString)
                .Must(a => DefaultValues.OptionValueArrayStartDelimiters.Any(a.StartsWith))
                .WithErrorCode("CLUAV0001")
                .WithMessage("Array values must begin with a valid delimiter");


            // array must end with a valid delimiter
            RuleFor(a => a.ArrayString)
                .Must(a => DefaultValues.OptionValueArrayEndDelimiters.Any(a.EndsWith))
                .WithErrorCode("CLUAV0002")
                .WithMessage("Array values must end with a valid delimiter");

           // array delimiter start and end indices match
           RuleFor(a => a.ArrayString)
               .Must(n =>
                   DefaultValues.OptionValueArrayStartDelimiters.Select((s, i) => new {i, s})
                       .Where(t => n.StartsWith(t.s))
                       .Select(t => t.i)
                       .FirstOrDefault()
                   ==
                   DefaultValues.OptionValueArrayEndDelimiters.Select((s, i) => new {i, s})
                       .Where(t => n.EndsWith(t.s))
                       .Select(t => t.i)
                       .FirstOrDefault())
               .WithErrorCode("CLUAV0003")
               .WithMessage("Array values must open and close with matching delimiters");

           // array must contain valid split delimiter
           RuleFor(a => a.ArrayString)
               .Must(n => DefaultValues.OptionValueArrayDelimiters.Any(n.Contains))
               .WithErrorCode("CLUAV0004")
               .WithMessage("Array values must have valid separator delimiters");
        }

        [ExcludeFromCodeCoverage]
        public ValidationState DoValidate(IArgumentArrayValue item, ErrorCategory category)
        {
	        var v = new ArgumentArrayValueValidator();
	        var result = v.Validate(item);

	        return result.ConvertToValidationState(category);
        }

        [ExcludeFromCodeCoverage]
        public ValidationState DoValidate(IArgumentArrayValue item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
        {
	        if (ruleSets is null) DoValidate(item, defaultCategory);

	        var v = new ArgumentArrayValueValidator();
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