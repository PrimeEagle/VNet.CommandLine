using System;
using System.Collections.Generic;
using VNet.Validation;

namespace VNet.CommandLine
{
	public class Usage : IUsage
	{
		public IList<ICategory> Categories { get; init; }
		public IList<IVerb> Verbs { get; init; }
		public ValidationState Validity { get; init; }
		public Dictionary<Type, object> InstantiatedCategories { get; set; }

		public Usage()
		{
			this.Categories = new List<ICategory>();
			this.Verbs = new List<IVerb>();
			this.Validity = new ValidationState();
            this.InstantiatedCategories = new Dictionary<Type, object>();
        }
	}
}
