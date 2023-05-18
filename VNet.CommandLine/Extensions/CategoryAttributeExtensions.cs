using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
	public static class CategoryAttributeExtensions
	{
		public static Category ConvertToCategory(this ICategoryAttribute attribute)
		{
			var category = new Category(attribute.Name);
			category.FillBaseProperties(attribute);

			return category;
		}
	}
}