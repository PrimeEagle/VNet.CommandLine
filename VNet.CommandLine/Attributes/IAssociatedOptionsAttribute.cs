using System.Collections.Generic;

namespace VNet.CommandLine.Attributes
{
	public interface IAssociatedOptionsAttribute : IAttribute
	{
		IEnumerable<string> OptionNames { get; set; }
	}
}
