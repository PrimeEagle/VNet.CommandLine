using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    [ExcludeFromCodeCoverage]
    public class NullOptionValidator : IValidator<IOption>
    {
	    public ValidationState DoValidate(IOption item, ErrorCategory category)
	    {
		    return new ValidationState();
	    }

	    public ValidationState DoValidate(IOption item, IEnumerable<string> ruleSets, ErrorCategory defaCategory)
	    {
		    return new ValidationState();
	    }
    }
}