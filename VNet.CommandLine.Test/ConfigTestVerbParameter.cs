using System;
using System.Collections.Generic;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Test
{
    public class ConfigTestVerbParameter
    {
        public string Name { get; init; }
        public string[] AlternateNames { get; init; }
        public string[] PropertyNames { get; init; }
        public object[] PropertyValues { get; init; }
        public bool DefaultVerb { get; init; }
        public bool HelpVerb { get; init; }
        public bool VersionVerb { get; init; }
        public IEnumerable<ConditionAttribute> Conditions { get; set; }

        public ConfigTestVerbParameter()
        {
            this.Conditions = new List<ConditionAttribute>();
        }
    }
}