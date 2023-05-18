namespace VNet.CommandLine
{
	public class ProcessingOptions
	{
		public bool LogConfigurationErrors { get; set; }
		public bool LogUsageErrors { get; set; }
		public bool DisplayHelpOnConfigurationErrors { get; set; }
		public bool DisplayHelpOnUsageErrors { get; set; }
		public bool DisplayConfigurationErrorMessages { get; set; }
		public bool DisplayUsageErrorMessages { get; set; }
		public bool StopIfConfigurationErrors { get; set; }
	}
}