using System;
using System.Collections.Generic;
using System.Reflection;

namespace VNet.CommandLine
{
	public interface ICondition
    {
        ConditionType ConditionType { get; set; }
        List<object> Values { get; set; }
        Type SourceBaseType { get; set; }
        MethodInfo SourceMethod { get; set; }
        PropertyInfo SourceProperty { get; set; }
        Type TypeToExecute { get; set; }
        MethodInfo MethodToExecute { get; set; }
        PropertyInfo PropertyForResult { get; set; }


		Condition Clone();
	}
}