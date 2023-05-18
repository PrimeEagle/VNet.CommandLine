using System.Collections.Generic;

namespace VNet.CommandLine
{
    public interface IAssociatedOptions
    {
        IEnumerable<string> Options { get; init; }
    }
}
