using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using VNet.Validation;
using VNet.Validation.FluentValidation;

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class ArgumentValuePairValidator : AbstractValidator<IArgumentValuePair>, VNet.Validation.IValidator<IArgumentValuePair>
    {
        public ArgumentValuePairValidator()
        {
            // value cannot contain more than one separator
            RuleFor(a => a.ArgumentValue)
                .Must(n => DefaultValues.OptionValueSeparators
                    .Count(n.Contains) <= 1)
                .WithErrorCode("CLUVP0001")
                .WithMessage("Value '{PropertyValue}' contains more than one separator");
        }

        [ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IArgumentValuePair item, ErrorCategory category)
        {
	        var v = new ArgumentValuePairValidator();
	        var result = v.Validate(item);

	        return result.ConvertToValidationState(category);
        }

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IArgumentValuePair item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
        {
	        if (ruleSets is null) DoValidate(item, defaultCategory);

	        var v = new ArgumentValuePairValidator();
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