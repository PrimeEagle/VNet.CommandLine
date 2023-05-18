namespace VNet.CommandLine
{
    public class DefaultExecutor : IExecutor
	{
		public void Execute(IUsage usage)
		{
			foreach (var verb in usage.Verbs.OrderForExecution())
			{
				if(!usage.InstantiatedCategories.ContainsKey(verb.SourceBaseType)) continue;
                
				var methodInfo = verb?.Method;
                var cat = usage.InstantiatedCategories[verb.SourceBaseType];
				methodInfo.Invoke(cat, null);
			}
		}
	}
}