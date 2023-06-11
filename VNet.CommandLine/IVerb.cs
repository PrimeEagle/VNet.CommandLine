using System.Collections.Generic;
using System.Reflection;

namespace VNet.CommandLine
{
	public interface IVerb : IBaseCommand
    {
	    int? ExecutionOrder { get; set; }
	    MethodInfo Method { get; set; }
        ICategory Category { get; set; }
	    List<string> AssociatedOptionNames { get; set; }
	    List<IOption> ArgumentOptions { get; set; }
    }
}
