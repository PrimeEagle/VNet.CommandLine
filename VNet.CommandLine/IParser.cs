using VNet.CommandLine.Validation;

namespace VNet.CommandLine
{
	public interface IParser
    {
        IUsage Parse(string[] args, IConfiguration configuration, ICommandLineValidatorManager validator);
    }
}
