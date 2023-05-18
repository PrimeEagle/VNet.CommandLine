using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine
{
    [ExcludeFromCodeCoverage]
    public class ParsedVerbsResult
    {
        public List<IVerb> Verbs { get; set; }
        public ValidationState ValidationState { get; set; }

        public ParsedVerbsResult()
        {
            this.ValidationState = new ValidationState();
        }
    }
}
