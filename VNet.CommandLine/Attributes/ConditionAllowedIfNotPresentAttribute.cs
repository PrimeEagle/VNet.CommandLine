// ReSharper disable ParameterTypeCanBeEnumerable.Local

namespace VNet.CommandLine.Attributes
{
    public class ConditionAllowedIfNotPresentAttribute : ConditionAttribute
    {
        public ConditionAllowedIfNotPresentAttribute(string[] names, ConditionOperator op = ConditionOperator.And) : base()
        {
            this.MethodToExecute = "DefaultConditionMethods.IfNotExists";
            this.PropertyForResult = "Allowed";
            this.Values = new object[]  { op, names };
        }
    }
}