using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VNet.Validation;

namespace VNet.CommandLine.Validation
{
    [ExcludeFromCodeCoverage]
    public class NullArgumentStartValidator : IValidator<IArgumentStart>
    {
        public ValidationState DoValidate(IArgumentStart item, ErrorCategory category)
        {
            return new ValidationState();
        }

        public ValidationState DoValidate(IArgumentStart item, IEnumerable<string> ruleSets, ErrorCategory defaCategory)
        {
            return new ValidationState();
        }
    }
}