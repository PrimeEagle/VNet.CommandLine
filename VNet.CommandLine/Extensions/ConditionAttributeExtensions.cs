using System;
using System.Linq;
using System.Reflection;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
    public static class ConditionAttributeExtensions
    {
        public static Condition ConvertToCondition(this IConditionAttribute attribute, Type sourceBaseType, MethodInfo sourceMethod, PropertyInfo sourceProperty)
        {
            Type executeClass;
            string executeMethod;

            if (attribute.MethodToExecute.Contains('.'))
            {
                var values = attribute.MethodToExecute.Split('.');

                executeClass = Type.GetType(values[0]);
                executeMethod = values[1];
            }
            else
            {
                executeClass = sourceBaseType;
                executeMethod = sourceMethod?.Name;
            }

            var condition = new Condition
            {
                ConditionType = attribute.ConditionType,
                Values = attribute.Values.ToList(),
                SourceBaseType = sourceBaseType,
                SourceMethod = sourceMethod,
                SourceProperty = sourceProperty,
                TypeToExecute = executeClass,
                MethodToExecute = (executeClass is not null && !string.IsNullOrEmpty(executeMethod)) ? executeClass.GetMethod(executeMethod) : null,
                PropertyForResult = sourceBaseType.GetProperty(attribute.PropertyForResult)
            };

            return condition;
        }
    }
}