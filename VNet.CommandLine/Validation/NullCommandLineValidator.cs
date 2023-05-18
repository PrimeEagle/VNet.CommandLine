using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    [ExcludeFromCodeCoverage]
    public class NullCommandLineValidator : IValidator<ICommandLine>
    {
	    public ValidationState DoValidate(ICommandLine item, ErrorCategory category)
	    {
		    return new ValidationState();
	    }

	    public ValidationState DoValidate(ICommandLine item, IEnumerable<string> ruleSets, ErrorCategory defaCategory)
	    {
		    return new ValidationState();
	    }
    }
}