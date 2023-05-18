namespace VNet.CommandLine
{
	public class Help
	{
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public int? DisplayOrder { get; set; }

		public Help Clone()
		{
			var clone = new Help();
			clone.DisplayName = this.DisplayName;
			clone.Description = this.Description;
			clone.DisplayOrder = this.DisplayOrder;

			return clone;
		}
	}
}
