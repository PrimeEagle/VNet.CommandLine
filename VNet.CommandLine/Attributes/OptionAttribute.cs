using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VNet.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class OptionAttribute : Attribute, IOptionAttribute
    {
	    public string Name { get; set; }
	    public string[] AlternateNames { get; set; }
	    public string[] AllNames
	    {
		    get
		    {
			    var result = new List<string> { this.Name.ToLower() };
			    result.AddRange(this.AlternateNames.Select(n => n.ToLower()));

			    return result.ToArray();
		    }
	    }
		public bool Required { get; set; }
	    public bool Allowed { get; set; }
	    public Type DataType { get; set; }
	    public bool NeedsValue { get; set; }
	    public bool AllowDuplicates { get; set; }
	    public bool AllowNullValue { get; set; }

	    public OptionAttribute(string name, string[] alternateNames, Type dataType)
	    {
		    this.Name = name ?? string.Empty;
		    this.AlternateNames = alternateNames ?? Array.Empty<string>();
		    this.DataType = dataType ?? typeof(string);
		    this.Allowed = true;
	    }

		public OptionAttribute(string name)
        {
            this.Name = name ?? string.Empty;
            this.AlternateNames = Array.Empty<string>();
			this.Allowed = true;
			this.DataType = typeof(string);
        }
    }
}