using System.Collections.Generic;

namespace VNet.CommandLine
{
    public interface IArgumentStart
    {
	    IOption Option { get; set; }
	    IEnumerable<IVerb> Verbs { get; set; }
	    IVerb DefaultVerb { get; set; }
    }
}
