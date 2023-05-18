using System.Linq;
// ReSharper disable ParameterTypeCanBeEnumerable.Local

namespace VNet.CommandLine.Attributes
{
    public class ConditionAllowedIfPresentAttribute : ConditionAttribute
    {
        public ConditionAllowedIfPresentAttribute(string[] names, ConditionOperator op = ConditionOperator.And) : base()
        {
            this.MethodToExecute = "DefaultConditionMethods.IfExists";
            this.PropertyForResult = "Allowed";
            this.Values = new object[] { op, names };
        }
	}
}