using System;
using System.Linq;
using System.Reflection;
using VNet.CommandLine.Attributes;
using VNet.CommandLine.Extensions;
using VNet.Testing;
using VNet.Utility.Extensions;


namespace VNet.CommandLine
{
	public class DefaultLoader : ILoader
	{
		public IConfiguration Load(object[] parameters)
		{
			var assembly = (IAssembly)parameters[0];

			return LoadConfiguration(assembly);
		}

		private static IConfiguration LoadConfiguration(IAssembly assembly)
		{
			var configuration = new Configuration(assembly);

			foreach (var c in assembly.CategoryClasses())
			{
				var category = LoadCategory(configuration, c);

				foreach (var v in c.VerbMethods())
				{
					var verb = LoadVerb(configuration, v);
					verb.Category = category;
					category.Verbs.Add(verb);
				}

				foreach (var o in c.OptionProperties())
				{
					var option = LoadOption(configuration, o);
					option.Category = category;
					category.Options.Add(option);
				}

				configuration.Categories.Add(category);
			}

			return configuration;
		}

		private static Category LoadCategory(IConfiguration configuration, Type c)
		{
			var catAttr = c.Attributes<CategoryAttribute>().First();

			var category = catAttr.ConvertToCategory();
			category.SourceBaseType = c;
			category.Conditions.AddRange(c.CategoryConditionAttributes().Select(s => s.ConvertToCondition(c, null, null)).ToList());
			category.Help = c.CategoryHelpAttribute().ConvertToHelp();
			category.Configuration = configuration;
			category.Attributes.Add(catAttr);
			category.Attributes.AddRange(c.CategoryConditionAttributes());

			var helpAttribute = (HelpAttribute)c.CategoryHelpAttribute();
			if (helpAttribute is not null) category.Attributes.Add(helpAttribute);

			return category;
		}

		private static Verb LoadVerb(IConfiguration configuration, MethodInfo mi)
		{
			var verbAttr = mi.VerbAttributes().First();
			var verb = verbAttr.ConvertToVerb();
			verb.SourceBaseType = mi.DeclaringType;
			verb.Method = mi;
			verb.Conditions.AddRange(mi.VerbConditionAttributes().Select(c => c.ConvertToCondition(mi.DeclaringType, mi, null)).ToList());
			verb.Help = mi.VerbHelpAttribute().ConvertToHelp();
			verb.Configuration = configuration;
			verb.Attributes.Add(verbAttr);
			verb.Attributes.AddRange(mi.VerbConditionAttributes());
			var helpAttribute = (HelpAttribute)mi.VerbHelpAttribute();
			if (helpAttribute is not null) verb.Attributes.Add(helpAttribute);

			var defaultVerbAttribute = (DefaultVerbAttribute)mi.VerbDefaultVerbAttribute();
			if (defaultVerbAttribute is not null) verb.Attributes.Add(defaultVerbAttribute);

			var assocOptionsAttribute = (AssociatedOptionsAttribute)mi.VerbAssociatedOptionsAttribute();
			if (assocOptionsAttribute is not null)
			{
				verb.Attributes.Add(assocOptionsAttribute);
				verb.AssociatedOptionNames = assocOptionsAttribute.OptionNames.ToList();
			}

			return verb;
		}

		private static Option LoadOption(IConfiguration configuration, PropertyInfo pi)
		{
			var optAttr = pi.OptionAttributes().First();
			var option = optAttr.ConvertToOption();
			option.SourceBaseType = pi.DeclaringType;
			option.Property = pi;
			option.Conditions.AddRange(pi.OptionConditionAttributes().Select(c => c.ConvertToCondition(pi.DeclaringType, null, pi)).ToList());
			option.Help = pi.OptionHelpAttribute().ConvertToHelp();
			option.Configuration = configuration;
			option.Attributes.Add(optAttr);
			option.Attributes.AddRange(pi.OptionConditionAttributes());
			var helpAttribute = (HelpAttribute)pi.OptionHelpAttribute();
			if (helpAttribute is not null) option.Attributes.Add(helpAttribute);

			return option;
		}
	}
}