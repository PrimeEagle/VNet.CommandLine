namespace VNet.CommandLine
{
	public class CommandLine : ICommandLine
	{
		public IConfiguration Configuration { get; init; }
		public IUsage Usage { get; init; }

		public CommandLine(IConfiguration configuration, IUsage usage)
		{
			this.Configuration = configuration;
			this.Usage = usage;
		}
	}
}
