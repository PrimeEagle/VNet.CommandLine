using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using VNet.CommandLine.Attributes;
using VNet.Testing;
using VNet.Utility;
using VNet.Utility.Extensions;

// ReSharper disable InconsistentNaming

namespace VNet.CommandLine.Test
{
    [ExcludeFromCodeCoverage]
    public class FakeAssemblyWrapper : IAssembly
    {
        public Assembly Assembly { get; init; }
        private readonly ModuleBuilder _moduleBuilder;

        public FakeAssemblyWrapper()
        {
            var assemblyName = new AssemblyName("FakeAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name + ".dll");

            this.Assembly = assemblyBuilder;
            _moduleBuilder = moduleBuilder;
        }

        public Type CreateClassTest(ConfigTestCategoryParameter category, IEnumerable<ConfigTestVerbParameter> verbs,
            IEnumerable<ConfigTestOptionParameter> options, bool removeReservedVerbs)
        {
            // create the class
            var categoryAttributes = new List<CustomAttributeBuilder>();

            var categoryAttrConstructor =
                typeof(CategoryAttribute).GetConstructors().First(c => c.GetParameters().Length == 1);
            var categoryAttributeBuilder =
                new CustomAttributeBuilder(categoryAttrConstructor, new object[] { category.Name });
            categoryAttributes.Add(categoryAttributeBuilder);

            foreach (var con in category.Conditions)
            {
                CustomAttributeBuilder conditionBuilder;
                switch (con)
                {
                    case ConditionAllowedIfPresentAttribute:
                    case ConditionAllowedIfNotPresentAttribute:
                    case ConditionRequiredIfPresentAttribute:
                    case ConditionRequiredIfNotPresentAttribute:
                        conditionBuilder = new CustomAttributeBuilder(typeof(ConditionAttribute).GetConstructors()
                                .First(c => c.GetParameters().Length == 2),
                            new object[] { con.Values.Skip(1).Take(con.Values.Length - 2).ToArray(), con.Values[0] });
                        break;
                    default:
                        conditionBuilder = new CustomAttributeBuilder(typeof(ConditionAttribute).GetConstructors()
                                .First(c => c.GetParameters().Length == 4),
                            new object[] { con.MethodToExecute, con.PropertyForResult, con.ConditionType, con.Values });
                        break;
                }

                categoryAttributes.Add(conditionBuilder);
            }

            var tb = ReflectionUtility.CreateClass(_moduleBuilder, category.Name, TypeAttributes.Class,
                categoryAttributes);



            if (verbs is not null)
            {
                foreach (var verb in verbs)
                {
                    var verbAttributeList = new List<CustomAttributeBuilder>();


                    // get properties for VerbAttribute
                    var piListVerb = new List<PropertyInfo>();
                    foreach (var name in verb.PropertyNames)
                    {
                        if (!name.IsValidPropertyName<VerbAttribute>())
                            throw new InvalidCastException($"Property name '{name}' not valid for type VerbAttribute");

                        piListVerb.Add(typeof(VerbAttribute).GetProperty(name));
                    }


                    // Verb attribute
                    var verbAttrConstructor = typeof(VerbAttribute)
                        .GetConstructors()
                        .FirstOrDefault(c => c.GetParameters().Length == 2);

                    if (verbAttrConstructor is null)
                        throw new ArgumentException(
                            $"No matching constructor was found for VerbAttribute. Expected constructor with 2 arguments.");

                    var verbAttributeBuilder = new CustomAttributeBuilder(verbAttrConstructor,
                        new object[] { verb.Name, verb.AlternateNames }, piListVerb.ToArray(), verb.PropertyValues);
                    verbAttributeList.Add(verbAttributeBuilder);

                    if (options is not null)
                    {
                        var associatedOptionNames = options.Select(n => n.Name).ToArray();

                        var assocOptionsAttrConstructor = typeof(AssociatedOptionsAttribute)
                            .GetConstructors()
                            .FirstOrDefault(c => c.GetParameters().Length == 1);

                        if (assocOptionsAttrConstructor is null)
                            throw new ArgumentException(
                                $"No matching constructor was found for AssociatedOptionsAttribute. Expected constructor with 1 arguments.");

                        var assocOptionsAttributeBuilder = new CustomAttributeBuilder(assocOptionsAttrConstructor,
                            new object[] { associatedOptionNames.ToArray() });
                        verbAttributeList.Add(assocOptionsAttributeBuilder);
                    }

                    if (verb.DefaultVerb)
                    {
                        var defAttrConstructor = typeof(DefaultVerbAttribute)
                            .GetConstructors()
                            .FirstOrDefault(c => c.GetParameters().Length == 0);

                        if (defAttrConstructor is null)
                            throw new ArgumentException(
                                $"No matching constructor was found for DefaultVerbAttribute. Expected constructor with 0 arguments.");

                        var defAttributeBuilder = new CustomAttributeBuilder(defAttrConstructor, Array.Empty<object>());
                        verbAttributeList.Add(defAttributeBuilder);
                    }

                    if (verb.HelpVerb)
                    {
                        var helpAttrConstructor = typeof(HelpVerbAttribute)
                            .GetConstructors()
                            .FirstOrDefault(c => c.GetParameters().Length == 0);

                        if (helpAttrConstructor is null)
                            throw new ArgumentException(
                                $"No matching constructor was found for HelpVerbAttribute. Expected constructor with 0 arguments.");

                        var helpAttributeBuilder = new CustomAttributeBuilder(helpAttrConstructor, Array.Empty<object>());
                        verbAttributeList.Add(helpAttributeBuilder);
                    }

                    if (verb.VersionVerb)
                    {
                        var versionAttrConstructor = typeof(VersionVerbAttribute)
                            .GetConstructors()
                            .FirstOrDefault(c => c.GetParameters().Length == 0);

                        if (versionAttrConstructor is null)
                            throw new ArgumentException(
                                $"No matching constructor was found for VersionVerbAttribute. Expected constructor with 0 arguments.");

                        var versionAttributeBuilder =
                            new CustomAttributeBuilder(versionAttrConstructor, Array.Empty<object>());
                        verbAttributeList.Add(versionAttributeBuilder);
                    }

                    foreach (var con in verb.Conditions)
                    {
                        CustomAttributeBuilder conditionBuilder;
                        switch (con)
                        {
                            case ConditionAllowedIfPresentAttribute:
                            case ConditionAllowedIfNotPresentAttribute:
                            case ConditionRequiredIfPresentAttribute:
                            case ConditionRequiredIfNotPresentAttribute:
                                conditionBuilder = new CustomAttributeBuilder(typeof(ConditionAttribute).GetConstructors()
                                        .First(c => c.GetParameters().Length == 2),
                                    new object[] { con.Values.Skip(1).Take(con.Values.Length - 2).ToArray(), con.Values[0] });
                                break;
                            default:
                                conditionBuilder = new CustomAttributeBuilder(typeof(ConditionAttribute).GetConstructors()
                                        .First(c => c.GetParameters().Length == 4),
                                    new object[]
                                        {con.MethodToExecute, con.PropertyForResult, con.ConditionType, con.Values});
                                break;
                        }

                        verbAttributeList.Add(conditionBuilder);
                    }

                    tb.AddMethod(verb.Name,
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        null,
                        null,
                        null,
                        null,
                        verbAttributeList);
                }
            }

            if (options is not null)
            {
                foreach (var option in options)
                {
                    var optionAttributes = new List<CustomAttributeBuilder>();

                    // get properties for OptionAttribute
                    var piListOption = new List<PropertyInfo>();
                    foreach (var name in option.PropertyNames)
                    {
                        if (!name.IsValidPropertyName<OptionAttribute>())
                            throw new InvalidCastException(
                                $"Property name '{name}' not valid for type OptionAttribute");

                        piListOption.Add(typeof(OptionAttribute).GetProperty(name));
                    }

                    var attrConstructor = typeof(OptionAttribute)
                        .GetConstructors()
                        .FirstOrDefault(c => c.GetParameters().Length == 3);

                    if (attrConstructor is null)
                        throw new ArgumentException(
                            $"No matching constructor was found for OptionAttribute. Expected constructor with 3 arguments.");

                    var customAttributeBuilder = new CustomAttributeBuilder(attrConstructor,
                        new object[] { option.Name, option.AlternateNames, option.DataType }, piListOption.ToArray(),
                        option.PropertyValues);
                    optionAttributes.Add(customAttributeBuilder);

                    var propertyName = option.Name;

                    foreach (var con in option.Conditions)
                    {
                        CustomAttributeBuilder conditionBuilder;
                        switch (con)
                        {
                            case ConditionAllowedIfPresentAttribute:
                            case ConditionAllowedIfNotPresentAttribute:
                            case ConditionRequiredIfPresentAttribute:
                            case ConditionRequiredIfNotPresentAttribute:
                                conditionBuilder = new CustomAttributeBuilder(typeof(ConditionAttribute)
                                        .GetConstructors()
                                        .First(c => c.GetParameters().Length == 2),
                                    new object[]
                                        {con.Values.Skip(1).Take(con.Values.Length - 2).ToArray(), con.Values[0]});
                                break;
                            default:
                                conditionBuilder = new CustomAttributeBuilder(typeof(ConditionAttribute)
                                        .GetConstructors()
                                        .First(c => c.GetParameters().Length == 4),
                                    new object[]
                                        {con.MethodToExecute, con.PropertyForResult, con.ConditionType, con.Values});
                                break;
                        }

                        optionAttributes.Add(conditionBuilder);
                    }

                    tb.AddProperty(propertyName,
                        option.DataType,
                        FieldAttributes.Public,
                        optionAttributes);
                }
            }

            return tb.CreateType();
        }

        public Type CreateClass(string className, IEnumerable<FakeAttributeParams> attributes)
        {
            // create the class
            var categoryAttrConstructor = typeof(CategoryAttribute).GetConstructors().First(c => c.GetParameters().Length == 1);
            var categoryAttributeBuilder = new CustomAttributeBuilder(categoryAttrConstructor, new object[] { className });
            var tb = ReflectionUtility.CreateClass(_moduleBuilder, className, TypeAttributes.Class, new List<CustomAttributeBuilder> { categoryAttributeBuilder });

            if (attributes == null) return tb.CreateType();

            var faList = attributes.ToList();


            // add verb method
            if (faList.Count(a => a.AttributeType == typeof(VerbAttribute)) > 1)
                throw new ArgumentException("No more than one VerbAttribute can be used at one time.");

            var faVerb = faList.FirstOrDefault(a => a.AttributeType == typeof(VerbAttribute));
            if (faVerb is not null)
            {
                var attributeList = new List<CustomAttributeBuilder>();


                // get properties for VerbAttribute
                var piListVerb = new List<PropertyInfo>();
                foreach (var name in faVerb.PropertyNames)
                {
                    if (!name.IsValidPropertyName<VerbAttribute>())
                        throw new InvalidCastException($"Property name '{name}' not valid for type VerbAttribute");

                    piListVerb.Add(faVerb.AttributeType.GetProperty(name));
                }


                // Verb attribute
                var verbAttrConstructor = typeof(VerbAttribute)
                    .GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == faVerb.ConstructorArgs.Length);

                if (verbAttrConstructor is null) throw new ArgumentException($"No matching constructor was found for VerbAttribute. Expected constructor with {faVerb.ConstructorArgs.Length} arguments.");

                var verbAttributeBuilder = new CustomAttributeBuilder(verbAttrConstructor, faVerb.ConstructorArgs, piListVerb.ToArray(), faVerb.PropertyValues);
                attributeList.Add(verbAttributeBuilder);


                // AssociatedOptions attribute
                var faAssociatedOption = faList.FirstOrDefault((a => a.AttributeType == typeof(OptionAttribute)));
                var associatedOptionNames = faList.Where(a => a.AttributeType == typeof(OptionAttribute))
                    .Select(n => n.ConstructorArgs[0]).ToArray();
                if (faAssociatedOption is not null)
                {
                    var assocOptionsAttrConstructor = typeof(AssociatedOptionsAttribute)
                        .GetConstructors()
                        .FirstOrDefault(c => c.GetParameters().Length == 1);

                    if (assocOptionsAttrConstructor is null)
                        throw new ArgumentException(
                            $"No matching constructor was found for AssociatedOptionsAttribute. Expected constructor with 1 arguments.");

                    var assocOptionsAttributeBuilder = new CustomAttributeBuilder(assocOptionsAttrConstructor,
                        new object[] { associatedOptionNames.ToArray() });
                    attributeList.Add(assocOptionsAttributeBuilder);
                }

                // DefaultVerb attribute
                var faDefaultVerb = faList.FirstOrDefault(a => a.AttributeType == typeof(DefaultVerbAttribute));
                if (faDefaultVerb is not null)
                {
                    // get properties for DefaultVerbAttribute
                    var piDefaultVerb = new List<PropertyInfo>();
                    foreach (var name in faDefaultVerb.PropertyNames)
                    {
                        if (!name.IsValidPropertyName<DefaultVerbAttribute>())
                            throw new InvalidCastException($"Property name '{name}' not valid for type DefaultVerbAttribute");

                        piDefaultVerb.Add(faDefaultVerb.AttributeType.GetProperty(name));
                    }

                    var defAttrConstructor = typeof(DefaultVerbAttribute)
                        .GetConstructors()
                        .FirstOrDefault(c => c.GetParameters().Length == 0);

                    if (defAttrConstructor is null) throw new ArgumentException($"No matching constructor was found for DefaultVerbAttribute. Expected constructor with 0 arguments.");

                    var defAttributeBuilder = new CustomAttributeBuilder(defAttrConstructor, Array.Empty<object>(), piDefaultVerb.ToArray(), faDefaultVerb.PropertyValues);
                    attributeList.Add(defAttributeBuilder);
                }

                tb.AddMethod(faVerb.ConstructorArgs[0].ToString(),
                    MethodAttributes.Public | MethodAttributes.Virtual,
                         null,
                      null,
                                  null,
                                  null,
                                  attributeList);
            }

            var faHelpVerb = faList.FirstOrDefault(a => a.AttributeType == typeof(HelpVerbAttribute));
            if (faHelpVerb is not null)
            {
                // get properties for HelpVerbAttribute
                var piListHelpVerb = new List<PropertyInfo>();
                foreach (var name in faHelpVerb.PropertyNames)
                {
                    if (!name.IsValidPropertyName<HelpVerbAttribute>())
                        throw new InvalidCastException($"Property name '{name}' not valid for type HelpVerbAttribute");

                    piListHelpVerb.Add(faHelpVerb.AttributeType.GetProperty(name));
                }

                var attrConstructor = typeof(HelpVerbAttribute)
                    .GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == 0);

                if (attrConstructor is null)
                    throw new ArgumentException(
                        $"No matching constructor was found for HelpVerbAttribute. Expected constructor with 0 arguments.");


                var customAttributeBuilder = new CustomAttributeBuilder(attrConstructor, faHelpVerb.ConstructorArgs, piListHelpVerb.ToArray(), faHelpVerb.PropertyValues);


                tb.AddMethod("HelpVerb",
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    null,
                    null,
                    null,
                    null,
                    new List<CustomAttributeBuilder>() { customAttributeBuilder });
            }


            var faVersionVerb = faList.FirstOrDefault(a => a.AttributeType == typeof(VersionVerbAttribute));
            if (faVersionVerb is not null)
            {
                // get properties for VersionVerbAttribute
                var piListVersionVerb = new List<PropertyInfo>();
                foreach (var name in faVersionVerb.PropertyNames)
                {
                    if (!name.IsValidPropertyName<VersionVerbAttribute>())
                        throw new InvalidCastException($"Property name '{name}' not valid for type VersionVerbAttribute");

                    piListVersionVerb.Add(faVersionVerb.AttributeType.GetProperty(name));
                }

                var attrConstructor = typeof(VersionVerbAttribute)
                    .GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == 0);

                if (attrConstructor is null)
                    throw new ArgumentException(
                        $"No matching constructor was found for VersionVerbAttribute. Expected constructor with 0 arguments.");


                var customAttributeBuilder = new CustomAttributeBuilder(attrConstructor, faVersionVerb.ConstructorArgs, piListVersionVerb.ToArray(), faVersionVerb.PropertyValues);


                tb.AddMethod("VersionVerb",
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    null,
                    null,
                    null,
                    null,
                    new List<CustomAttributeBuilder>() { customAttributeBuilder });
            }

            foreach (var faOption in faList.Where((a => a.AttributeType == typeof(OptionAttribute))))
            {
                // get properties for OptionAttribute
                var piListOption = new List<PropertyInfo>();
                foreach (var name in faOption.PropertyNames)
                {
                    if (!name.IsValidPropertyName<OptionAttribute>())
                        throw new InvalidCastException($"Property name '{name}' not valid for type OptionAttribute");

                    piListOption.Add(faOption.AttributeType.GetProperty(name));
                }

                var attrConstructor = faOption.AttributeType
                    .GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == faOption.ConstructorArgs.Length);

                if (attrConstructor is null) throw new ArgumentException($"No matching constructor was found for OptionAttribute. Expected constructor with {faOption.ConstructorArgs.Length} arguments.");

                var customAttributeBuilder = new CustomAttributeBuilder(attrConstructor, faOption.ConstructorArgs, piListOption.ToArray(), faOption.PropertyValues);

                var propertyName = faOption.ConstructorArgs[0].ToString();

                tb.AddProperty(propertyName,
                    (Type)faOption.ConstructorArgs[2],
                    FieldAttributes.Public,
                    new List<CustomAttributeBuilder>() { customAttributeBuilder });
            }

            return tb.CreateType();
        }

        public Type[] GetTypes()
        {
            return this.Assembly.GetTypes();
        }
    }
}