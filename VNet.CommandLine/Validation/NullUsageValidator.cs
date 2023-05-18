using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    [ExcludeFromCodeCoverage]
    public class NullUsageValidator : IValidator<IUsage>
    {
	    public ValidationState DoValidate(IUsage item, ErrorCategory category)
	    {
		    return new ValidationState();
	    }

	    public ValidationState DoValidate(IUsage item, IEnumerable<string> ruleSets, ErrorCategory defaCategory)
	    {
		    return new ValidationState();
	    }
    }
}