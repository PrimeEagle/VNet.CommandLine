using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
	public static class VerbAttributeExtensions
	{
		public static Verb ConvertToVerb(this IVerbAttribute attribute)
		{
			var verb = new Verb(attribute.Name);
			verb.FillBaseProperties(attribute);
			verb.ExecutionOrder = attribute.ExecutionOrder;

			return verb;
		}
	}
}