using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VNet.CommandLine
{
    [ExcludeFromCodeCoverage]
    public class AssociatedOptions : IAssociatedOptions
    {
        public IEnumerable<string> Options { get; init; }

        public AssociatedOptions(IEnumerable<string> options)
        {
            this.Options = options;
        }
    }
}
