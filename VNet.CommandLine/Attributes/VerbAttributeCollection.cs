using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VNet.CommandLine.Attributes
{
    [ExcludeFromCodeCoverage]
    public class VerbAttributeCollection : IVerbAttributeCollection
    {
        public IEnumerable<IVerbAttribute> VerbAttributes { get; init; }

        public VerbAttributeCollection(IEnumerable<IVerbAttribute> verbAttributes)
        {
            this.VerbAttributes = verbAttributes;
        }
    }
}
