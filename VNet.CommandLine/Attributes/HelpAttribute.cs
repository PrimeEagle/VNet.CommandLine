using System;
using System.Diagnostics.CodeAnalysis;
// ReSharper disable MemberCanBePrivate.Global

namespace VNet.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    [ExcludeFromCodeCoverage]
    public class HelpAttribute : Attribute, IHelpAttribute
    {
        public string DisplayName { get; set; }
        public string DisplayText { get; set; }
        public int? DisplayOrder { get; set; }

        public HelpAttribute(string displayName, string displayText)
        {
	        this.DisplayName = displayName;
	        this.DisplayText = displayText;
        }

        public HelpAttribute(string displayName, string displayText, int displayOrder)
        {
	        this.DisplayName = displayName;
	        this.DisplayText = displayText;
	        this.DisplayOrder = displayOrder;
        }
    }
}