using System.Collections.Generic;
using VNet.Testing;

namespace VNet.CommandLine
{
	public interface IConfiguration
    {
        IAssembly AssemblyWrapper { get; init; }
        IVerb DefaultVerb { get; }
        IList<ICategory> Categories { get; init; }
    }
}
