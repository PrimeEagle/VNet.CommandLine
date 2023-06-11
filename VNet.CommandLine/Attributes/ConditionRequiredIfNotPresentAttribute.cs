
// ReSharper disable ParameterTypeCanBeEnumerable.Local

namespace VNet.CommandLine.Attributes
{
    public class ConditionRequiredIfNotPresentAttribute : ConditionAttribute
    {
        public ConditionRequiredIfNotPresentAttribute(string[] names, ConditionOperator op = ConditionOperator.And) : base()
        {
            this.MethodToExecute = "DefaultConditionMethods.IfExists";
            this.PropertyForResult = "Required";
            this.Values = new object[] { op, names };
        }
    }
}