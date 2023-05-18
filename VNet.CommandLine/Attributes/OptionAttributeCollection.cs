using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VNet.CommandLine.Attributes
{
    [ExcludeFromCodeCoverage]
    public class OptionAttributeCollection : IOptionAttributeCollection
    {
        public IEnumerable<IOptionAttribute> OptionAttributes { get; init; }

        public OptionAttributeCollection(IEnumerable<IOptionAttribute> optionAttributes)
        {
            this.OptionAttributes = optionAttributes;
        }
    }
}
