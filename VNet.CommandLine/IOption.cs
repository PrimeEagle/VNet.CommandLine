using System;
using System.Reflection;

namespace VNet.CommandLine
{
	public interface IOption : IBaseCommand
	{
		Type DataType { get; set; }
		object DataValue { get; set; }
		bool NeedsValue { get; set; }
		bool AllowDuplicates { get; set; }
		bool AllowNullValue { get; set; }
		ICategory Category { get; set; }
		PropertyInfo Property { get; set; }
		IVerb ArgumentVerb { get; set; }
	}
}
