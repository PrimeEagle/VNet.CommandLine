using System.Collections.Generic;

namespace VNet.CommandLine
{
    public interface ICategoryOptions
    {
        IEnumerable<IOption> Options { get; init; }
    }
}
