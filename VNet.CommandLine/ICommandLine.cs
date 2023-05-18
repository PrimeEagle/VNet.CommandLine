namespace VNet.CommandLine
{
	public interface ICommandLine
	{
		IConfiguration Configuration { get; init; }
		IUsage Usage { get; init; }
	}
}
