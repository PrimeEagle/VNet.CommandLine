using System.Linq;
// ReSharper disable ParameterTypeCanBeEnumerable.Local

namespace VNet.CommandLine.Attributes
{
    public class ConditionRequiredIfPresentAttribute : ConditionAttribute
    {
        public ConditionRequiredIfPresentAttribute(string[] names, ConditionOperator op = ConditionOperator.And) : base()
        {
            this.MethodToExecute = "DefaultConditionMethods.IfExists";
            this.PropertyForResult = "Required";
            this.Values = new object[] { op, names };
        }
    }
}