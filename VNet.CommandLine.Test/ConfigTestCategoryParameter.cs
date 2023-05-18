using System;
using System.Collections.Generic;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Test
{
    public class ConfigTestCategoryParameter
    {
        public string Name { get; init; }
        public string[] AlternateNames { get; init; }
        public string[] PropertyNames { get; init; }
        public object[] PropertyValues { get; init; }
        public IEnumerable<ConditionAttribute> Conditions { get; set; }

        public ConfigTestCategoryParameter()
        {
            this.Conditions = new List<ConditionAttribute>();
        }
    }
}
