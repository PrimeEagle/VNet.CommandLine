using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
	public static class HelpAttributeExtensions
	{
		public static Help ConvertToHelp(this IHelpAttribute attribute)
		{
			var help = new Help();

			return help;
		}
	}
}