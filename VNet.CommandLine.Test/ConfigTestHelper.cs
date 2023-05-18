using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VNet.CommandLine.Attributes;
using VNet.Testing;

namespace VNet.CommandLine.Test
{
    [ExcludeFromCodeCoverage]
    internal static class ConfigTestHelper
    {
        internal static void AddVerb(IAssembly assembly,
            string className,
            string nameVerb = null, string alternateNameVerb = null,
            string nameOpt = null, string alternateNameOpt = null,
            string[] verbPropertyNames = null, object[] verbPropertyValues = null,
            string[] optPropertyNames = null, object[] optPropertyValues = null,
            Type optDataType = null,
            bool removeReservedVerbs = false)
        {
            AddVerb(assembly,
                className,
                nameVerb, alternateNameVerb is null ? null : new string[] { alternateNameVerb },
                nameOpt, alternateNameOpt is null ? null : new string[] { alternateNameOpt },
                verbPropertyNames, verbPropertyValues,
                optPropertyNames, optPropertyValues,
                optDataType, removeReservedVerbs);
        }

        internal static void AddTest(IAssembly assembly, string className,
            string verbName = null, string verbAlternateName = null,
            string optName = null, string optAlternateName = null,
            bool removeReservedVerbs = false)
        {
            var verbAlternateNames = new string[] {verbAlternateName};
            var optAlternateNames = new string[] { optAlternateName };

            AddTest(assembly, className, verbName, verbAlternateNames, optName, optAlternateNames, removeReservedVerbs);
        }

        internal static void AddTest(IAssembly assembly, string className,
            string verbName = null, string[] verbAlternateNames = null,
            string optName = null, string[] optAlternateNames = null,
            bool removeReservedVerbs = false)
        {
            var category = new ConfigTestCategoryParameter()
            {
                Name = className
            };

            var verb = new ConfigTestVerbParameter()
            {
                Name = verbName,
                AlternateNames = verbAlternateNames
            };

            var option = new ConfigTestOptionParameter()
            {
                Name = optName,
                AlternateNames = optAlternateNames
            };

            var options = new List<ConfigTestOptionParameter> {option};

            AddTest(assembly, category, verb, options, removeReservedVerbs);
        }

        internal static void AddTest(IAssembly assembly, ConfigTestCategoryParameter category,
                            ConfigTestVerbParameter verb, IEnumerable<ConfigTestOptionParameter> options = null,
                            bool removeReservedVerbs = false)
        {
            var fakeAssembly = assembly as FakeAssemblyWrapper;
            var verbs = new List<ConfigTestVerbParameter> { verb };

            fakeAssembly?.CreateClassTest(category, verbs, options, removeReservedVerbs);
        }

        internal static void AddVerb(IAssembly assembly,
                                                    string className,
                                                    string verbName = null, string[] verbAlternateNames = null,
                                                    string optName = null, string[] optAlternateNames = null,
                                                    string[] verbPropertyNames = null, object[] verbPropertyValues = null,
                                                    string[] optPropertyNames = null, object[] optPropertyValues = null,
                                                    Type optDataType = null, bool removeReservedVerbs = false)
        {
            var fakeAssembly = assembly as FakeAssemblyWrapper;
            var attrList = new List<FakeAttributeParams>();

            var sName = verbName ?? string.Empty;

            var verb = new FakeAttributeParams
            {
                AttributeType = typeof(VerbAttribute),
                ConstructorArgs = new object[] { sName, verbAlternateNames },
                PropertyNames = verbPropertyNames ?? Array.Empty<string>(),
                PropertyValues = verbPropertyValues ?? Array.Empty<object>()
            };
            attrList.Add(verb);

            var sNameOpt = optName ?? string.Empty;

            if (optName is not null)
            {
                var opt = new FakeAttributeParams
                {
                    AttributeType = typeof(OptionAttribute),
                    ConstructorArgs = new object[] { sNameOpt, optAlternateNames, optDataType ?? typeof(string) },
                    PropertyNames = optPropertyNames ?? Array.Empty<string>(),
                    PropertyValues = optPropertyValues ?? Array.Empty<object>()
                };

                attrList.Add(opt);
            }

            var help = new FakeAttributeParams
            {
                AttributeType = typeof(HelpVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var version = new FakeAttributeParams
            {
                AttributeType = typeof(VersionVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            if (!removeReservedVerbs)
            {
                attrList.Add(help);
                attrList.Add(version);
            }

            fakeAssembly?.CreateClass(className, attrList);
        }

        internal static void AddSingleVerbWithDefaultVerb(IAssembly assembly,
                                                         string className,
                                                         string nameVerb = null, string alternateNameVerb = null,
                                                         string nameOpt = null, string alternateNameOpt = null,
                                                         bool removeReservedVerbs = false)
        {
            var attrList = new List<FakeAttributeParams>();
            var fakeAssembly = assembly as FakeAssemblyWrapper;

            var vAlternateName = string.IsNullOrEmpty(alternateNameVerb)
                ? Array.Empty<string>()
                : new string[] { alternateNameVerb };

            var oAlternateName = string.IsNullOrEmpty(alternateNameOpt)
                ? Array.Empty<string>()
                : new string[] { alternateNameOpt };
            var verb = new FakeAttributeParams
            {
                AttributeType = typeof(VerbAttribute),
                ConstructorArgs = new object[] { nameVerb, vAlternateName },
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var opt = new FakeAttributeParams
            {
                AttributeType = typeof(OptionAttribute),
                ConstructorArgs = new object[] { nameOpt, oAlternateName, typeof(string) },
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var def = new FakeAttributeParams
            {
                AttributeType = typeof(DefaultVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var help = new FakeAttributeParams
            {
                AttributeType = typeof(HelpVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var version = new FakeAttributeParams
            {
                AttributeType = typeof(VersionVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            attrList.Add(verb);
            attrList.Add(opt);
            attrList.Add(def);

            if (!removeReservedVerbs)
            {
                attrList.Add(help);
                attrList.Add(version);
            }

            fakeAssembly?.CreateClass(className, attrList);
        }

        internal static void AddOptionWithoutVerb(IAssembly assembly,
                                                     string className,
                                                     string nameOpt = null, string alternateNameOpt = null)
        {
            var attrList = new List<FakeAttributeParams>();
            var fakeAssembly = assembly as FakeAssemblyWrapper;

            var optAlternateName = string.IsNullOrEmpty(alternateNameOpt) ? Array.Empty<string>() : new string[] { alternateNameOpt };

            var opt = new FakeAttributeParams
            {
                AttributeType = typeof(OptionAttribute),
                ConstructorArgs = new object[] { nameOpt, optAlternateName, typeof(string) },
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            attrList.Add(opt);

            fakeAssembly?.CreateClass(className, attrList);
        }


        internal static void AddSingleVerbForDefaultVerbs(IAssembly assembly,
            string className,
            bool includeHelp, bool includeVersion,
            string verbName = null, string alternateName = null,
            string[] propertyNames = null, object[] propertyValues = null)
        {
            var fakeAssembly = assembly as FakeAssemblyWrapper;

            var attrList = new List<FakeAttributeParams>();

            if (verbName is not null || alternateName is not null)
            {
                var vAlternateName = string.IsNullOrEmpty(alternateName)
                    ? Array.Empty<string>()
                    : new string[] { alternateName };

                var verb = new FakeAttributeParams
                {
                    AttributeType = typeof(VerbAttribute),
                    ConstructorArgs = new object[] { verbName ?? string.Empty, vAlternateName },
                    PropertyNames = propertyNames ?? Array.Empty<string>(),
                    PropertyValues = propertyValues ?? Array.Empty<object>()
                };
                attrList.Add(verb);
            }

            var help = new FakeAttributeParams
            {
                AttributeType = typeof(HelpVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var version = new FakeAttributeParams
            {
                AttributeType = typeof(VersionVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            if (includeHelp) attrList.Add(help);
            if (includeVersion) attrList.Add(version);

            fakeAssembly?.CreateClass(className, attrList);
        }


        internal static void AddSingleVerbWithMultipleOptions(IAssembly assembly,
                                                                    string className,
                                                                    string nameVerb, string alternateNameVerb,
                                                                    string[] verbPropertyNames, object[] verbPropertyValues,
                                                                    IEnumerable<ConfigTestOptionParameter> optionParameters,
                                                                    bool removeReservedVerbs = false)
        {
            var fakeAssembly = assembly as FakeAssemblyWrapper;

            var verb = new FakeAttributeParams
            {
                AttributeType = typeof(VerbAttribute),
                ConstructorArgs = new object[] { nameVerb, new string[] { alternateNameVerb } },
                PropertyNames = verbPropertyNames ?? Array.Empty<string>(),
                PropertyValues = verbPropertyValues ?? Array.Empty<object>()
            };

            var attrList = optionParameters.Select(option => new FakeAttributeParams
            {
                AttributeType = typeof(OptionAttribute),
                ConstructorArgs = new object[] { option.Name, option.AlternateNames, option.DataType ?? typeof(string) },
                PropertyNames = option.PropertyNames ?? Array.Empty<string>(),
                PropertyValues = option.PropertyValues ?? Array.Empty<object>()
            })
                .ToList();

            var help = new FakeAttributeParams
            {
                AttributeType = typeof(HelpVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var version = new FakeAttributeParams
            {
                AttributeType = typeof(VersionVerbAttribute),
                ConstructorArgs = Array.Empty<object>(),
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            attrList.Add(verb);


            if (!removeReservedVerbs)
            {
                attrList.Add(help);
                attrList.Add(version);
            }

            fakeAssembly?.CreateClass(className, attrList);
        }

        internal static void CreateValidConfiguration(IAssembly assembly)
        {
            ConfigTestHelper.AddVerb(assembly, "CopyCommand", "c", "copy", "s", "source", new string[] { "Required" }, new object[] { true });
            ConfigTestHelper.AddVerb(assembly, "MoveCommand", "m", "move", "t", "target", null, null, new string[] { "Required" }, new object[] { true }, null, true);
            ConfigTestHelper.AddVerb(assembly, "InfoCommand", "i", "info", "d", "details", null, null, new string[] { "AllowDuplicates" }, new object[] { true }, null, true);
            ConfigTestHelper.AddVerb(assembly, "ExtractCommand", "e", "extract", null, null, null, null, null, null, null, true);

            var saveOptions = new List<ConfigTestOptionParameter>();

            var option1 = new ConfigTestOptionParameter()
            {
                Name = "so1",
                AlternateNames = new string[] { "saveoption1" },
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var option2 = new ConfigTestOptionParameter()
            {
                Name = "so2",
                AlternateNames = new string[] { "saveoption2" },
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var option3 = new ConfigTestOptionParameter()
            {
                Name = "so3",
                AlternateNames = new string[] { "saveoption3" },
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var option4 = new ConfigTestOptionParameter()
            {
                Name = "so4",
                AlternateNames = new string[] { "saveoption4" },
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var option5 = new ConfigTestOptionParameter()
            {
                Name = "so5",
                AlternateNames = new string[] { "saveoption5" },
                PropertyNames = Array.Empty<string>(),
                PropertyValues = Array.Empty<object>()
            };

            var option6 = new ConfigTestOptionParameter()
            {
                Name = "so6",
                AlternateNames = new string[] { "saveoption6" },
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, true }
            };

            var option7 = new ConfigTestOptionParameter()
            {
                Name = "so7",
                AlternateNames = new string[] { "saveoption7" },
                PropertyNames = new string[] { "NeedsValue", "AllowDuplicates" },
                PropertyValues = new object[] { true, true }
            };

            var optionChar = new ConfigTestOptionParameter()
            {
                Name = "char",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(char),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionString1 = new ConfigTestOptionParameter()
            {
                Name = "string1",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(string),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionString2 = new ConfigTestOptionParameter()
            {
                Name = "string2",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(String),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionBool = new ConfigTestOptionParameter()
            {
                Name = "bool",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(bool),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };


            var optionDateTime = new ConfigTestOptionParameter()
            {
                Name = "datetime",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(DateTime),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionInt = new ConfigTestOptionParameter()
            {
                Name = "int",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(int),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionShort = new ConfigTestOptionParameter()
            {
                Name = "short",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(short),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionLong = new ConfigTestOptionParameter()
            {
                Name = "long",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(long),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionInt16 = new ConfigTestOptionParameter()
            {
                Name = "int16",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(Int16),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionInt32 = new ConfigTestOptionParameter()
            {
                Name = "int32",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(Int32),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionInt64 = new ConfigTestOptionParameter()
            {
                Name = "int64",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(Int64),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionSingle = new ConfigTestOptionParameter()
            {
                Name = "single",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(Single),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionDouble = new ConfigTestOptionParameter()
            {
                Name = "double",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(double),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionDecimal = new ConfigTestOptionParameter()
            {
                Name = "decimal",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(decimal),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionFloat = new ConfigTestOptionParameter()
            {
                Name = "float",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(float),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionEnum = new ConfigTestOptionParameter()
            {
                Name = "enum",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(ConditionOperator),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue" },
                PropertyValues = new object[] { true, false }
            };

            var optionArray = new ConfigTestOptionParameter()
            {
                Name = "array",
                AlternateNames = Array.Empty<string>(),
                DataType = typeof(Array),
                PropertyNames = new string[] { "NeedsValue", "AllowNullValue", "AllowDuplicates" },
                PropertyValues = new object[] { true, false, true }
            };

            saveOptions.Add(option1);
            saveOptions.Add(option2);
            saveOptions.Add(option3);
            saveOptions.Add(option4);
            saveOptions.Add(option5);
            saveOptions.Add(option6);
            saveOptions.Add(option7);
            saveOptions.Add(optionChar);
            saveOptions.Add(optionString1);
            saveOptions.Add(optionString2);
            saveOptions.Add(optionBool);
            saveOptions.Add(optionDateTime);
            saveOptions.Add(optionInt);
            saveOptions.Add(optionShort);
            saveOptions.Add(optionLong);
            saveOptions.Add(optionInt16);
            saveOptions.Add(optionInt32);
            saveOptions.Add(optionInt64);
            saveOptions.Add(optionSingle);
            saveOptions.Add(optionDouble);
            saveOptions.Add(optionDecimal);
            saveOptions.Add(optionFloat);
            saveOptions.Add(optionEnum);
            saveOptions.Add(optionArray);

            ConfigTestHelper.AddSingleVerbWithMultipleOptions(assembly, "SaveCommand", "sv", "save", null, null, saveOptions, true);
        }
    }
}