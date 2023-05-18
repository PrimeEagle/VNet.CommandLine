using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    [ExcludeFromCodeCoverage]
    public class NullArgumentArrayValueValidator : IValidator<IArgumentArrayValue>
    {
	    public ValidationState DoValidate(IArgumentArrayValue item, ErrorCategory category)
	    {
		    return new ValidationState();
	    }

	    public ValidationState DoValidate(IArgumentArrayValue item, IEnumerable<string> ruleSets, ErrorCategory defaCategory)
	    {
		    return new ValidationState();
	    }
	}
}