using System.Collections.Generic;

namespace VNet.CommandLine
{
    public interface ICategoryVerbs
    {
        IEnumerable<IVerb> Verbs { get; init; }
    }
}
