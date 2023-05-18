using System.Collections.Generic;

namespace VNet.CommandLine.Attributes
{
    public interface IVerbAttributeCollection : IAttribute
    {
        IEnumerable<IVerbAttribute> VerbAttributes { get; init; }
    }
}
