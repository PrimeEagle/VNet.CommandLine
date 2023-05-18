namespace VNet.CommandLine.Attributes
{
	public interface IBaseAttribute : IAttribute
	{
		string Name { get; set; }
		string[] AlternateNames { get; set; }
		string[] AllNames { get; }
		bool Required { get; set; }
	}
}