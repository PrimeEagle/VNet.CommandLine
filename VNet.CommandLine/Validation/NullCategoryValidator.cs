using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    [ExcludeFromCodeCoverage]
    public class NullCategoryValidator : IValidator<ICategory>
    {
	    public ValidationState DoValidate(ICategory item, ErrorCategory category)
	    {
		    return new ValidationState();
	    }

	    public ValidationState DoValidate(ICategory item, IEnumerable<string> ruleSets, ErrorCategory defaCategory)
	    {
		    return new ValidationState();
	    }
    }
}