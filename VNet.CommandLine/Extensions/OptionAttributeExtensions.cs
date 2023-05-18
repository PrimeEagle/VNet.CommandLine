using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
	public static class OptionAttributeExtensions
	{
		public static Option ConvertToOption(this IOptionAttribute attribute)
		{
			var option = new Option(attribute.Name);
			option.FillBaseProperties(attribute);
			option.AllowDuplicates = attribute.AllowDuplicates;
			option.AllowNullValue = attribute.AllowNullValue;
			option.DataType = attribute.DataType;
			option.NeedsValue = attribute.NeedsValue;
			
			return option;
		}
	}
}