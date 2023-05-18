using System.Collections.Generic;

namespace VNet.CommandLine
{
    public class ArgumentStart : IArgumentStart
    {
        public IOption Option { get; set; }
        public IEnumerable<IVerb> Verbs { get; set; }
        public IVerb DefaultVerb { get; set; }
    }
}
