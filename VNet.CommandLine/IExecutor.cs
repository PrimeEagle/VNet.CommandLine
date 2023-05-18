using VNet.CommandLine.Validation;

namespace VNet.CommandLine
{
	public interface IExecutor
    {
        void Execute(IUsage arguments);
    }
}