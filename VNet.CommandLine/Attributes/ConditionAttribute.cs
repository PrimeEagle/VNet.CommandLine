using System;

namespace VNet.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property,
		Inherited = true,
		AllowMultiple = true)]
    public class ConditionAttribute : Attribute, IConditionAttribute
    {
        public ConditionType ConditionType { get; set; }
        public object[] Values { get; set; }
        public string MethodToExecute { get; init; }
        public string PropertyForResult { get; init; }

        protected ConditionAttribute()
        {

        }

        public ConditionAttribute(string method, string property, ConditionType conditionType)
        {
            this.MethodToExecute = method;
            this.PropertyForResult = property;
            this.ConditionType = conditionType;
            this.Values = Array.Empty<object>();
        }

        public ConditionAttribute(string method, string property, ConditionType conditionType, object[] values)
        {
            this.MethodToExecute = method;
            this.PropertyForResult = property;
            this.ConditionType = ConditionType;
            this.Values = values;
        }
    }
}