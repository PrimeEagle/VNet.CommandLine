using VNet.Validation;

namespace VNet.CommandLine.Validation
{
	public interface ICommandLineValidatorParameters : IValidatorParameters
    {
	    IValidator<ICommandLine> CommandLineValidator { get; set; }
    }
}
