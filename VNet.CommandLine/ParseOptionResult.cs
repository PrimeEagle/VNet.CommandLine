using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine
{
    [ExcludeFromCodeCoverage]
    internal class ParseOptionResult
    {
        internal IEnumerable<string> UnknownOptions { get; set; }
        internal int LastIndexProcessed { get; set; }
        internal ValidationState ValidationState { get; set; }

        public ParseOptionResult()
        {
            this.ValidationState = new ValidationState();
        }
    }
}
