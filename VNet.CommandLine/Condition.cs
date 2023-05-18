using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VNet.CommandLine
{
	public class Condition : ICondition
	{
		public ConditionType ConditionType { get; set; }
		public List<object> Values { get; set; }
		public Type SourceBaseType { get; set; }
		public MethodInfo SourceMethod { get; set; }
		public PropertyInfo SourceProperty { get; set; }
		public Type TypeToExecute { get; set; }
		public MethodInfo MethodToExecute { get; set; }
		public PropertyInfo PropertyForResult { get; set; }

		public Condition Clone()
		{
			var clone = new Condition
			{
				ConditionType = this.ConditionType,
                Values = new List<object>(this.Values.ToList()),
				SourceBaseType = this.SourceBaseType,
                SourceMethod = this.SourceMethod,
                SourceProperty = this.SourceProperty,
				TypeToExecute = this.TypeToExecute,
				MethodToExecute = this.MethodToExecute,
				PropertyForResult = this.PropertyForResult
			};

			return clone;
		}
	}
}