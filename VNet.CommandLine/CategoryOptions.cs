using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VNet.CommandLine
{
    [ExcludeFromCodeCoverage]
    public class CategoryOptions : ICategoryOptions
    {
        public IEnumerable<IOption> Options { get; init; }

        public CategoryOptions(IEnumerable<IOption> options)
        {
            this.Options = options;
        }
    }
}
