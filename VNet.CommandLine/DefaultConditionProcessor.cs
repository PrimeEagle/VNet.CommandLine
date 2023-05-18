using VNet.CommandLine.Extensions;

namespace VNet.CommandLine
{
    public class DefaultConditionProcessor : IConditionProcessor
    {
        public IUsage Process(IConfiguration configuration, IUsage usage)
        {
            foreach (var c in usage.Categories)
            {
                ((BaseCommand)c).ApplyConditions(configuration, usage);
            }

            foreach (var v in usage.Verbs)
            {
                ((BaseCommand)v).ApplyConditions(configuration, usage);

                foreach (var o in v.ArgumentOptions)
                {
                    ((BaseCommand)o).ApplyConditions(configuration, usage);
                }
            }
            return usage;
        }
    }
}