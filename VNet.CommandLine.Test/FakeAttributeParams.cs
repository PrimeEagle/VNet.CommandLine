using System;
using System.Collections.Generic;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Test
{
    public class FakeAttributeParams
    {
        public Type AttributeType { get; init; }
        public object[] ConstructorArgs { get; init; }
        public string[] PropertyNames { get; init; }
        public object[] PropertyValues { get; init; }
        public IEnumerable<ConditionAttribute> Conditions { get; init; }

        public FakeAttributeParams()
        {
            this.Conditions = new List<ConditionAttribute>();
        }
    }
}
