using VNet.Validation;

namespace VNet.CommandLine.Validation
{
	public interface ICommandLineValidatorManager : IValidatorManager
    {
        ICommandLineValidatorParameters ValidatorParameters { get; init; }
    }
}
