using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using VNet.Utility.Extensions;
using VNet.Validation;
using VNet.Validation.FluentValidation;

// ReSharper disable RedundantAssignment

namespace VNet.CommandLine.Validation.FluentValidation
{
	public class CommandLineValidator : AbstractValidator<ICommandLine>, VNet.Validation.IValidator<ICommandLine>
	{
		public CommandLineValidator()
		{
			RuleSet("Configuration", () =>
			{
				//var errorTag = this.GetType().Name.ToUpper() + "C";
				//var i = 0;

				RuleFor(cl => cl.Configuration).SetValidator(new ConfigurationValidator(), new string[] { "Configuration" });
			});

			RuleSet("Usage", () =>
			{
				var errorTag = this.GetType().Name.ToUpper() + "U";
				var i = 0;

				RuleFor(cl => cl.Usage).SetValidator(new UsageValidator(), new string[] { "Usage" });

				// required categories must be present in arguments
				RuleFor(c => c)
					.Must(v => v.Configuration.Categories
						.Where(v => v.Required)
						.Select(n => n.Name)
						.AllExistsIn(v.Usage.Verbs.Select(a => a.Name))
					)
					.Unless(c => !c.Configuration.CategoryVerbs().Any())
					.WithName("Required")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Required verb is missing");

				// required verbs must be present in arguments
				RuleFor(c => c)
					.Must(v => v.Configuration.CategoryVerbs()
						.Where(v => v.Required)
						.Select(n => n.Name)
						.AllExistsIn(v.Usage.Verbs.Select(a => a.Name))
					)
					.Unless(c => !c.Configuration.CategoryVerbs().Any())
					.WithName("Required")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Required verb is missing");

				// required options must be present in arguments
				RuleFor(c => c)
					.Must(v => v.Configuration.CategoryOptions()
						.Where(o => o.Required)
						.Where(o => v.Usage.Verbs.SelectMany(v => v.AssociatedOptionNames).Contains(o.Name))
						.Select(n => n.Name)
						.AllExistsIn(v.Usage.Verbs.SelectMany(a => a.ArgumentOptions.Select(b => b.Name)))
					)
					.Unless(c => !c.Configuration.CategoryOptions().Any())
					.WithName("Required")
					.WithErrorCode($"{errorTag}{(i++):###}")
					.WithMessage("Required option is missing");
			});
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(ICommandLine item, ErrorCategory category)
		{
			var v = new CommandLineValidator();
			var result = v.Validate(item);

			return result.ConvertToValidationState(category);
		}

		[ExcludeFromCodeCoverage]
		public ValidationState DoValidate(ICommandLine item, IEnumerable<string> ruleSets, ErrorCategory defaultCategory)
		{
			if (ruleSets is null) DoValidate(item, defaultCategory);

			var v = new CommandLineValidator();
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