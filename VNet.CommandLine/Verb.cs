using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable MemberCanBeProtected.Global

namespace VNet.CommandLine
{
	public class Verb : BaseCommand, IVerb
    {
	    public int? ExecutionOrder { get; set; }
        public MethodInfo Method { get; set; }
        public ICategory Category { get; set; }
        public List<string> AssociatedOptionNames { get; set; }
        public List<IOption> ArgumentOptions { get; set; }

        public Verb(string name)
        {
	        this.Name = name;
	        this.AssociatedOptionNames = new List<string>();
	        this.ArgumentOptions = new List<IOption>();
        }

        public override BaseCommand Clone()
        {
	        return new Verb(this);
        }

        protected Verb(Verb other) : base(other)
        {
	        this.ExecutionOrder = other.ExecutionOrder;
	        this.Method = other.Method;
	        this.Category = other.Category;
	        this.AssociatedOptionNames = new List<string>(other.AssociatedOptionNames.ToList());
	        this.ArgumentOptions = new List<IOption>(other.ArgumentOptions.Select(o => (IOption) o.Clone()));
        }
	}
}