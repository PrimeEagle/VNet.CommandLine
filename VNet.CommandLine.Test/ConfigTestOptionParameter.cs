using System;
using System.Collections.Generic;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Test
{
    public class ConfigTestOptionParameter
    {
        public string Name { get; init; }
        public string[] AlternateNames { get; init; }
        public string[] PropertyNames { get; init; }
        public object[] PropertyValues { get; init; }
        public Type DataType { get; init; }
        public IEnumerable<ConditionAttribute> Conditions { get; set; }

        public ConfigTestOptionParameter()
        {
            this.Conditions = new List<ConditionAttribute>();
        }
    }
}