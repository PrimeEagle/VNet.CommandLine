using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
	public static class HelpVerbAttributeExtensions
	{
		public static Verb ConvertToVerb(this IHelpVerbAttribute attribute)
		{
			var verb = new Verb(attribute.Name);

			return verb;
		}
	}
}