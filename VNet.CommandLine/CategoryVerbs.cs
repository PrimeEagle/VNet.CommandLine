using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VNet.CommandLine
{
    [ExcludeFromCodeCoverage]
    public class CategoryVerbs : ICategoryVerbs
    {
        public IEnumerable<IVerb> Verbs { get; init; }

        public CategoryVerbs(IEnumerable<IVerb> options)
        {
            this.Verbs = options;
        }
    }
}
