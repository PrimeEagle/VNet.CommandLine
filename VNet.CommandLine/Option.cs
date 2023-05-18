using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using VNet.Utility.Extensions;

namespace VNet.CommandLine
{
    public class Option : BaseCommand, IOption
    {
	    public Type DataType { get; set; }
        public object DataValue { get; set; }
        public bool NeedsValue { get; set; }
        public bool AllowDuplicates { get; set; }
        public bool AllowNullValue { get; set; }
        public ICategory Category { get; set; }
        [ExcludeFromCodeCoverage]
        public PropertyInfo Property { get; set; }
        public IVerb ArgumentVerb { get; set; }


        public Option(string name)
        {
	        this.Name = name;
        }

        public override BaseCommand Clone()
        {
	        return new Option(this);
        }

        protected Option(Option other) : base(other)
        {
	        this.DataType = other.DataType;
	        this.DataValue = other.DataValue;
	        this.NeedsValue = other.NeedsValue;
	        this.AllowDuplicates = other.AllowDuplicates;
	        this.AllowNullValue = other.AllowNullValue;
	        this.Category = other.Category;
	        this.Property = other.Property;
	        this.ArgumentVerb = other.ArgumentVerb;
        }
    }
}
