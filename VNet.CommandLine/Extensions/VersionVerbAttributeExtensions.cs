using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
	public static class VersionVerbAttributeExtensions
	{
		public static Verb ConvertToVerb(this IVersionVerbAttribute attribute)
		{
			var verb = new Verb(attribute.Name);

			return verb;
		}
	}
}