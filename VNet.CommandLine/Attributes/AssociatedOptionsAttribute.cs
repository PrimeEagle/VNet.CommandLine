using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VNet.CommandLine.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[ExcludeFromCodeCoverage]
	public class AssociatedOptionsAttribute : Attribute, IAssociatedOptionsAttribute
	{
		public IEnumerable<string> OptionNames { get; set; }

		// ReSharper disable once ParameterTypeCanBeEnumerable.Local
		public AssociatedOptionsAttribute(string[] optionNames)
		{
			this.OptionNames = optionNames;
		}
	}
}
