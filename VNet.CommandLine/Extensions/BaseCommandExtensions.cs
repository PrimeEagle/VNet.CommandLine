using System.Linq;
using VNet.CommandLine.Attributes;

namespace VNet.CommandLine.Extensions
{
    public static class BaseCommandExtensions
    {
        public static IBaseCommand FillBaseProperties(this IBaseCommand baseCommand, IBaseAttribute baseAttribute)
        {
            baseCommand.Name = baseAttribute.Name;
            baseCommand.AlternateNames.Clear();
            baseCommand.AlternateNames.AddRange(baseAttribute.AlternateNames.ToList());
            baseCommand.Allowed = true;
            baseCommand.Required = baseAttribute.Required;

            return baseCommand;
        }

        public static void ApplyConditions(this BaseCommand baseCommand, IConfiguration configuration, IUsage usage)
        {
            if (baseCommand is null) return;

            foreach (var cnd in baseCommand.Conditions)
            {
                if (cnd?.MethodToExecute == null ||
                    !usage.InstantiatedCategories.ContainsKey(cnd.TypeToExecute)) continue;

                var cat = usage.InstantiatedCategories[cnd.TypeToExecute];

                var result = cnd.TypeToExecute
                    ?.GetMethod(cnd?.MethodToExecute?.Name)
                    ?.Invoke(cat, new object[] {configuration, usage, cnd.Values})!;

                cnd.PropertyForResult?.SetValue(cat, result);
            }
        }
    }
}