using System;
using System.Collections.Generic;
using VNet.Validation;

namespace VNet.CommandLine
{
	public interface IUsage
    {
	    ValidationState Validity { get; init; }
        IList<ICategory> Categories { get; init; }
        IList<IVerb> Verbs { get; init; }
        Dictionary<Type, object> InstantiatedCategories { get; set; }
    }
}
