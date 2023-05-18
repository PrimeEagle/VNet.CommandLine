using System;
using System.Collections.Generic;
using System.Linq;
using VNet.CommandLine.Extensions;
using VNet.Utility.Extensions;

namespace VNet.CommandLine
{
    public static class DefaultConditionMethods
	{
        public static bool IfExists(IConfiguration configuration, IUsage usage, object[] parameters)
        {
            if (parameters is null || parameters.Length < 2) throw new ArgumentException();

            var op = (ConditionOperator)parameters[0];
            var names = (IEnumerable<string>)parameters[1];

            var allNames = usage.UsageCategoryNames()
                .Concat(usage.UsageVerbNames())
                .Concat(usage.UsageOptionNames());

            var result = op switch
            {
                ConditionOperator.And => names.AllExistsIn(allNames),
                ConditionOperator.Or => names.Any(a => allNames.Contains(a)),
                _ => false
            };

            return result;
        }

        public static bool IfNotExists(IConfiguration configuration, IUsage usage, object[] parameters)
        {
            if (parameters is null || parameters.Length < 2) throw new ArgumentException();

            var op = (ConditionOperator)parameters[0];
            var names = (IEnumerable<string>)parameters[1];

            var allNames = usage.UsageCategoryNames()
                .Concat(usage.UsageVerbNames())
                .Concat(usage.UsageOptionNames());

            var result = op switch
            {
                ConditionOperator.And => names.All(n => !allNames.Contains(n)),
                ConditionOperator.Or => names.Any(a => !allNames.Contains(a)),
                _ => false
            };

            return result;
        }
    }
}