using System.Collections.Generic;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    public class DefaultCommandLineValidatorParameters : DefaultValidatorParameters, ICommandLineValidatorParameters
    {
	    public IValidator<ICommandLine> CommandLineValidator { get; set; }

        public DefaultCommandLineValidatorParameters(IValidator<ICommandLine> commandLineValidator, List<IValidator> additionalValidators)
        {
	        this.CommandLineValidator = commandLineValidator;
            this.AdditionalValidators = additionalValidators ?? new List<IValidator>();
        }
    }
}