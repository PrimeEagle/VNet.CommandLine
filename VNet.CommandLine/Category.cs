using System;
using System.Collections.Generic;
using System.Linq;

namespace VNet.CommandLine
{
	public class Category : BaseCommand, ICategory
	{
		public List<IVerb> Verbs { get; set; }
		public List<IOption> Options { get; set; }

		public Category(string name)
		{
			this.Name = name;
			this.Verbs = new List<IVerb>();
			this.Options = new List<IOption>();
		}

		public Category(string name, string[] alternateNames, Type sourceBaseType)
		{
			this.Name = name;
			this.AlternateNames = alternateNames.ToList();
			this.SourceBaseType = sourceBaseType;
			this.Verbs = new List<IVerb>();
			this.Options = new List<IOption>();
		}

		public override BaseCommand Clone()
		{
			return new Category(this);
		}

		protected Category(Category other) : base(other)
		{
			this.Verbs = new List<IVerb>(other.Verbs.Select(v => (IVerb)v.Clone()));
			this.Options = new List<IOption>(other.Options.Select(o => (IOption)o.Clone()));
		}
	}
}