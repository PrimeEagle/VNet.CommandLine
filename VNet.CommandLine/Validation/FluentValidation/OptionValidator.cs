using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.Utility;
using VNet.Validation;
using VNet.Validation.FluentValidation;
// ReSharper disable RedundantAssignment

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class OptionValidator : AbstractValidator<IOption>, VNet.Validation.IValidator<IOption>
    {
        public OptionValidator()
        {
	        RuleSet("Configuration", () => {
		        var errorTag = this.GetType().Name.ToUpper() + "C";
                var i = 0;

				// options must have a valid data type
				RuleFor(opt => opt.DataType)
					.Must(o => DefaultValues.AllowedDataTypes.Contains(o) || o.IsArray || o.IsEnum)
					.Unless(o => o.DataType is null)
					.WithName("Data Type")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("{PropertyName} for option is not valid");

				// name not empty
				RuleFor(opt => opt.Name)
					.NotEmpty()
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot be empty");

				// alternate name not empty
				RuleForEach(opt => opt.AlternateNames)
					.Must(a => !string.IsNullOrEmpty(a))
					.When(o => o.AlternateNames.Any())
					.Unless(o => o.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot be empty");

				//// option cannot depend on itself
				//RuleFor(opt => opt)
				//	.Must(o => !o.DependsOn.Contains(o.Name))
				//	.Unless(o => o.DependsOn is null)
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("An option with {PropertyName} cannot refer to itself");

				//// option cannot depend on a blank
				//RuleFor(opt => opt)
				//	.Must(o => !o.DependsOn.Any(string.IsNullOrEmpty))
				//	.Unless(o => o.DependsOn is null)
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("An option {PropertyName} cannot be blank");

				//// option cannot be exclusive with itself
				//RuleFor(opt => opt)
				//	.Must(o => !o.ExclusiveWith.Contains(o.Name))
				//	.Unless(o => o.ExclusiveWith is null)
				//	.WithName("Exclusive With")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("An option with {PropertyName} cannot refer to itself");

				//// option cannot be exclusive with a blank
				//RuleFor(opt => opt)
				//	.Must(o => !o.ExclusiveWith.Any(string.IsNullOrEmpty))
				//	.Unless(o => o.ExclusiveWith is null)
				//	.WithName("Exclusive With")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("An option {PropertyName} cannot be blank");

				// names cannot contain special characters
				RuleFor(opt => opt.Name)
					.Must(o => !DefaultValues.SpecialCharacters.Where(s => !string.IsNullOrEmpty(s)).Any(o.Contains))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot contain special characters");

				// alternate names cannot contain special characters
				RuleForEach(opt => opt.AlternateNames.Select(a => a))
					.Must(o => !DefaultValues.SpecialCharacters.Where(s => !string.IsNullOrEmpty(s)).Any(o.Contains))
					.Unless(o => o.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot contain special characters");

				// names cannot contain reserved verb names
				RuleFor(opt => opt.Name)
					.Must(o => !DefaultValues.ReservedVerbs.Any(s => !string.IsNullOrEmpty(s) && s == o))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot be a reserved verb name");

				// alternate names cannot contain reserved verb names
				RuleForEach(opt => opt.AlternateNames.Select(a => a))
					.Must(o => !DefaultValues.ReservedVerbs.Any(s => !string.IsNullOrEmpty(s) && s == o))
					.Unless(o => o.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot be a reserved verb name");

				// names cannot start with category prefixes
				RuleFor(opt => opt.Name)
					.Must(o => !DefaultValues.CategoryPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(o.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot start with a category prefix");

				// alternate names cannot start with category prefixes
				RuleForEach(opt => opt.AlternateNames.Select(a => a))
					.Must(o => !DefaultValues.CategoryPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(o.StartsWith))
					.Unless(o => o.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot start with a category prefix");

				// names cannot start with verb prefixes
				RuleFor(opt => opt.Name)
					.Must(o => !DefaultValues.VerbPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(o.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot start with a verb prefix");

				// alternate names cannot start with verb prefixes
				RuleForEach(opt => opt.AlternateNames.Select(a => a))
					.Must(o => !DefaultValues.VerbPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(o.StartsWith))
					.Unless(o => o.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot start with a verb prefix");

				// names cannot start with option prefixes
				RuleFor(opt => opt.Name)
					.Must(o => !DefaultValues.OptionPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(o.StartsWith))
					.WithName("Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot start with an option prefix");

				// alternate names cannot start with option prefixes
				RuleForEach(opt => opt.AlternateNames.Select(a => a))
					.Must(o => !DefaultValues.OptionPrefixes.Where(s => !string.IsNullOrEmpty(s)).Any(o.StartsWith))
					.Unless(o => o.AlternateNames is null)
					.WithName("Alternate Name")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option {PropertyName} cannot start with an option prefix");

				// option needs value in order to allow null value
				RuleFor(opt => opt)
					.Must(o => o.NeedsValue || !o.AllowNullValue)
					.WithName("NeedsValue and AllowNullValue")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option cannot allow null value if it does not need a value");

				// option name can't equal verb name
				//RuleFor(c => c)
				//	.Must(o => !o.Category.Configuration.CategoryVerbAllNames().Any(v => v == o.Name && !string.IsNullOrEmpty(o.Name)))
				//	.WithName("Name")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("{PropertyName} for option cannot be the same as a verb name");

				//// Everything in Option DependsOn must refer to existing names
				//RuleFor(c => c)
				//	.Must(o => o.DependsOn
				//		.AllExistsIn(o.Category.Configuration.CategoryOptions().Select(n => n.Name)))
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Option {PropertyName} refers to a Name that was not found");

				//// Everything in Option ExclusiveWith must refer to existing names
				//RuleFor(c => c)
				//	.Must(o => o.ExclusiveWith
				//		.AllExistsIn(o.Category.Configuration.CategoryOptions().Select(n => n.Name)))
				//	.WithName("Exclusive With")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Option {PropertyName} refers to a Name that was not found");
	        });

	        RuleSet("Usage", () => {
		        var errorTag = this.GetType().Name.ToUpper() + "U";
                var i = 0;

				// has a value
				RuleFor(opt => opt.DataValue)
					.NotEmpty()
					.When(o => o.NeedsValue && !o.AllowNullValue)
					.WithName("Data Value")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option is missing a value");

				// does not need a value
				RuleFor(opt => opt.DataValue)
					.Null()
					.Unless(o => o.NeedsValue)
					.WithName("Data Value")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option cannot have a value");

				// null value allowed when AllowNullValue = true
				RuleFor(opt => opt.DataValue)
					.NotNull()
					.Unless(o => o.AllowNullValue || !o.NeedsValue)
					.WithName("Data Value")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Option cannot have a null value");

				// must have valid data value for data type
				RuleFor(opt => opt)
					.Must(ValidDataType)
					.Unless(opt => opt.DataValue is null)
					.Unless(opt => opt.DataType is null)
					.WithName("Data Type")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Data value not valid for data type");

				//// dependencies must be present
				//RuleFor(opt => opt)
				//	.Must(o => o.DependsOn.AllExistsIn(o.ArgumentVerb.Usage.UsageOptionNames()))
				//	.Unless(o => o.DependsOn is null)
				//	.Unless(o => !o.DependsOn.Any())
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Option dependency is missing");

				//// exclusivities must not be present
				//RuleFor(opt => opt)
				//	.Must(o => o.ExclusiveWith.NoneExistsIn(o.ArgumentVerb.Usage.UsageOptionNames()))
				//	.Unless(o => o.ExclusiveWith is null)
				//	.Unless(o => !o.ExclusiveWith.Any())
				//	.WithName("Depends On")
				//	.WithErrorCode($"{errorTag}{(i++):###}")
				//	.WithMessage("Option exclusivity is present");
			});
        }

        [ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IOption item, ErrorCategory category)
        {
	        var v = new OptionValidator();
	        var result = v.Validate(item);

	        return result.ConvertToValidationState(category);
        }

        [ExcludeFromCodeCoverage]
		public ValidationState DoValidate(IOption item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
        {
	        if (ruleSets is null) DoValidate(item, defaultCategory);

	        var v = new OptionValidator();
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

        private bool ValidDataType(IOption opt)
        {
            var result = false;

            var val = opt.DataValue.ToString();

            if (opt.DataType == typeof(string) || opt.DataType == typeof(String))
            {
                result = true;
            }
            else if (opt.DataType == typeof(char))
            {
                if (char.TryParse(val, out _)) result = true;
            }
            else if (opt.DataType == typeof(bool))
            {
                if (bool.TryParse(val, out _)) result = true;
            }
            else if (opt.DataType == typeof(DateTime))
            {
                if (DateTime.TryParse(val, out _)) result = true;
            }
            else if ((opt.DataType == typeof(short) || opt.DataType == typeof(Int16)))
            {
                if (short.TryParse(val, out _)) result = true;
            }
            else if ((opt.DataType == typeof(int) || opt.DataType == typeof(Int32)))
            {
                if (int.TryParse(val, out _)) result = true;
            }
            else if ((opt.DataType == typeof(long) || opt.DataType == typeof(Int64)))
            {
                if (long.TryParse(val, out _)) result = true;
            }
            else if ((opt.DataType == typeof(float) || opt.DataType == typeof(Single)))
            {
                if (float.TryParse(val, out _)) result = true;
            }
            else if (opt.DataType == typeof(decimal))
            {
                if (decimal.TryParse(val, out _)) result = true;
            }
            else if (opt.DataType == typeof(double))
            {
                if (double.TryParse(val, out _)) result = true;
            }
            else if (opt.DataType.IsEnum)
            {
                var found = false;
                foreach (var e in Enum.GetValues(opt.DataType))
                {
                    if (!string.Equals(val, e.ToString(), StringComparison.CurrentCultureIgnoreCase)) continue;

                    found = true;
                    break;
                }

                if (found) result = true;
            }
            else if ((opt.DataValue.GetType().IsArray) && opt.DataType == typeof(Array))
            {
                result = true;
            }

            return result;
        }
    }
}