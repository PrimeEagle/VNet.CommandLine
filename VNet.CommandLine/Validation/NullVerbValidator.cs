using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    [ExcludeFromCodeCoverage]
    public class NullVerbValidator :IValidator<IVerb>
    {
	    public ValidationState DoValidate(IVerb item, ErrorCategory category)
	    {
		    return new ValidationState();
	    }

        public ValidationState DoValidate(IVerb item, IEnumerable<string> ruleSets, ErrorCategory defaCategory)
        {
            return new ValidationState();
        }
    }
}