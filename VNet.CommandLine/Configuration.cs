using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.CommandLine.Attributes;
using VNet.Testing;

namespace VNet.CommandLine
{
	public class Configuration : IConfiguration
	{
		[ExcludeFromCodeCoverage]
		public IAssembly AssemblyWrapper { get; init; }
		public IList<ICategory> Categories { get; init; }

		public IVerb DefaultVerb
		{
			get
			{
				return this.Categories
					.SelectMany(c => c.Verbs)
					.FirstOrDefault(v => v.Attributes.Any(a => a.GetType() == typeof(DefaultVerbAttribute)));
			}
		}

		public Configuration(IAssembly assembly)
		{
			this.AssemblyWrapper = assembly;
			this.Categories = new List<ICategory>();
		}
	}
}