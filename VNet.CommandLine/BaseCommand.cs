using System;
using System.Collections.Generic;
using System.Linq;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine
{
	public abstract class BaseCommand : IBaseCommand
    {
	    public string Name { get; set; }
	    public List<string> AlternateNames { get; set; }
	    public List<string> AllNames
	    {
		    get
		    {
			    var result = new List<string> { this.Name.ToLower() };
			    result.AddRange(this.AlternateNames.Select(n => n.ToLower()));

			    return result;
		    }
	    }
		public bool Required { get; set; }
		public bool Allowed { get; set; }
		public Help Help { get; set; }
        public List<ICondition> ConfigurationConditions
        {
            get
            {
                return this.Conditions.Where(c => c.ConditionType == ConditionType.Configuration).ToList();
            }
        }
        public List<ICondition> UsageConditions
        {
            get
            {
                return this.Conditions.Where(c => c.ConditionType == ConditionType.Usage).ToList();
            }
        }
		public List<ICondition> Conditions { get; set; }
		public IUsage Usage { get; set; }
		public IConfiguration Configuration { get; set; }
		public Type SourceBaseType { get; set; }
		public List<IAttribute> Attributes { get; set; }

		protected BaseCommand()
		{
			this.AlternateNames = new List<string>();
			this.Conditions = new List<ICondition>();
			this.Help = new Help();
			this.Attributes = new List<IAttribute>();
		}

		protected BaseCommand(BaseCommand other)
		{
			this.Name = other.Name;
			this.AlternateNames = new List<string>(other.AlternateNames.ToList());
			this.Required = other.Required;
			this.Allowed = other.Allowed;
			this.Help = (Help)other.Help.Clone();
			this.Conditions = new List<ICondition>(other.Conditions.Select(c => (ICondition)c.Clone()));
			this.Usage = other.Usage;
			this.Configuration = other.Configuration;
			this.SourceBaseType = other.SourceBaseType;
			this.Attributes = other.Attributes;
		}

		public abstract BaseCommand Clone();
    }
}
