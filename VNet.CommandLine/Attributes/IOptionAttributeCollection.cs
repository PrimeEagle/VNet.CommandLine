using System.Collections.Generic;

namespace VNet.CommandLine.Attributes
{
    public interface IOptionAttributeCollection : IAttribute
    {
        IEnumerable<IOptionAttribute> OptionAttributes { get; init; }
    }
}
