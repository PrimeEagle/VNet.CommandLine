using System.Collections.Generic;
using VNet.CommandLine.Validation.FluentValidation;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
	public class DefaultValidator : ICommandLineValidatorManager
    {
        public ICommandLineValidatorParameters ValidatorParameters { get; init; }

        public DefaultValidator(ICommandLineValidatorParameters validatorParameters)
        {
            ValidatorParameters = validatorParameters;
        }

        public ValidationState Validate(object commandLine, ValidationState parseResult)
        {
	        var result = new ValidationState();
	        var com = commandLine as ICommandLine;

            var commandLineValidator = (CommandLineValidator)ValidatorParameters.CommandLineValidator;

            var configurationResult = commandLineValidator.DoValidate(com, new List<string>() { "Configuration" }, ErrorCategory.Configuration);
            var usageResult = commandLineValidator.DoValidate(com, new List<string>() { "Usage" }, ErrorCategory.Usage);

            result.Append(parseResult);
            result.Append(configurationResult);
            result.Append(usageResult);

			return result;
        }
    }
}