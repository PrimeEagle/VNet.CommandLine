namespace VNet.CommandLine.Attributes
{
	public interface IHelpAttribute : IAttribute
    {
        string DisplayName { get; set; }
        string DisplayText { get; set; }
        int? DisplayOrder { get; set; }
    }
}