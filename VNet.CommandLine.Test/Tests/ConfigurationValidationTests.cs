using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNet.CommandLine.Validation;
using VNet.Testing;
// ReSharper disable NotAccessedField.Local

namespace VNet.CommandLine.Test.Tests
{
	[TestClass]
	public class ConfigurationValidationTests
	{
        private static TestServices _services;
        private static IAssembly _assembly;
        private static TextWriter _textWriter;
        private static List<TextWriter> _streams;
        private static ILogger _logger;
        private static ILoader _config;
        private static IParser _parse;
        private static IConditionProcessor _conditionProcessor;
        private static ICommandLineValidatorManager _validate;
        private static IDisplayer _display;
        private static IExecutor _executor;
		private CommandLineManager _cm;

		[ClassInitialize()]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Test ClassSetup")]
		public static void ClassSetup(TestContext context)
		{
			_services = ConfigTestServices.ConfigureServices();

			_textWriter = _services.ApplicationServiceProvider.GetService<TextWriter>();
			_streams = new List<TextWriter>
			{
				_textWriter
			};
            _logger = _services.ApplicationServiceProvider.GetService<ILogger<CommandLineManagerTests>>();
            _config = _services.ApplicationServiceProvider.GetService<ILoader>();
            _parse = _services.ApplicationServiceProvider.GetService<IParser>();
            _conditionProcessor = _services.ApplicationServiceProvider.GetService<IConditionProcessor>();
            _validate = _services.ApplicationServiceProvider.GetService<ICommandLineValidatorManager>();
            _display = _services.ApplicationServiceProvider.GetService<IDisplayer>();
            _executor = _services.ApplicationServiceProvider.GetService<IExecutor>();
		}

		[TestInitialize()]
		public void TestSetup()
		{
			_assembly = _services.AssemblyServiceProvider.GetService<IAssembly>();
			_cm = new CommandLineManager();
		}

		[TestMethod]
		public void ReservedVerbUsedName()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test", "h", Array.Empty<string>(), null, null, null, null, null, null, null, true);
			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void ReservedVerbUsedAlternateName()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test", "test123", "help", null, null, null, null, null, null, null, true);
			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void ReservedOptionUsedName()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test", "t", "test", "h");
			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}

		[TestMethod]
		public void ReservedOptionUsedAlternateName()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test", "t", "", "o", "h", null, null, null, null, null, true);
			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}

		[TestMethod]
		public void NonUniqueVerbNames()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test1", null, null, null, null, null, null, null, true);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t", "test2", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void NonUniqueVerbAlternateNames()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test", null, null, null, null, null, null, null, true);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t2", "test", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void NonUniqueVerbBothNames()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, null, true);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t", "test", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void EmptyVerbName()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "", "test", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void EmptyVerbAlternateNameSomeValid()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", new string[] { "test", "" }, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void EmptyVerbAlternateNameNoneValid()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", (string)null, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void EmptyVerbBothNames()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "", "", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}

		[TestMethod]
		public void EmptyOptionName()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", "", "extra", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void EmptyOptionAlternateNameSomeValid()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", new string[] { "test" }, "e", new string[] { "opt1", "" });

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void EmptyOptionAlternateNameNoneValid()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", "e", null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void EmptyOptionBothNames()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", "", "");

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}

		[TestMethod]
		public void NonUniqueOptionName()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", "e", "extra1");
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t2", "test2", "e", "extra2", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void NonUniqueOptionAlternateNames()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", "e1", "extra");
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t2", "test2", "e2", "extra", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void NonUniqueOptionBothNames()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", "e", "extra");
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t2", "test2", "e", "extra", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void SingleDefaultVerb()
		{
			ConfigTestHelper.AddSingleVerbWithDefaultVerb(_assembly, "Test1", "t1", "test1", "e", "extra");

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void MultipleDefaultVerbs()
		{
			ConfigTestHelper.AddSingleVerbWithDefaultVerb(_assembly, "Test1", "t1", "test1", "e1", "extra1");
			ConfigTestHelper.AddSingleVerbWithDefaultVerb(_assembly, "Test2", "t2", "test2", "e2", "extra2", true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionWithVerb()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", (string[])null, "o", null, null, null, null, null, null, false);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionWithoutVerb()
		{
			ConfigTestHelper.AddOptionWithoutVerb(_assembly, "Test1", "e", "extra");

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbDependencyRefersToSelf()
		{
            ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "t" } }, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbDependencyDoesNotReferToSelf()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "a" } }, null, null, null, true);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "a", "testa", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "t" } }, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDependencyRefersToSelf()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "testv", "t", "testo", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "t" } });

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionDependencyDoesNotReferToSelf()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "vtest", "t", "test", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "a" } });
			ConfigTestHelper.AddVerb(_assembly, "Test2", "x", null, "a", "testa", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void VerbExclusiityRefersToSelf()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "t" } }, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbExclusivityDoesNotReferToSelf()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "a" } }, null, null, null, true);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "a", "testa", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionExclusivityRefersToSelf()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "vtest", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "t" } });

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionExclusivityDoesNotReferToSelf()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "vtest", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "a" } });
			ConfigTestHelper.AddVerb(_assembly, "Test2", "x", null, "a", "testa", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void VerbDependencyHasValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "a" } }, null, null);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "a", "testa", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void VerbDependencyDoesNotHaveValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "a" } }, null, null);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "b", "testb", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbExclusivityHasValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "a" } }, null, null);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "a", "testa", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void VerbExclusivityDoesNotHaveValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "a" } }, null, null);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "b", "testb", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionDependencyHasValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "vtest", "t", "test", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "a" } });
			ConfigTestHelper.AddVerb(_assembly, "Test2", "x", null, "a", "testa", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDependencyDoesNotHaveValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "vtest", "t", "test", null, null, new string[] { "DependsOn" }, new object[] { new string[] { "a" } });
			ConfigTestHelper.AddVerb(_assembly, "Test2", "x", null, "b", "testb", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionExclusivityHasValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "vtest", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "a" } });
			ConfigTestHelper.AddVerb(_assembly, "Test2", "x", null, "a", "testa", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionExclusivityDoesNotHaveValidValue()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "vt", "vtest", "t", "test", null, null, new string[] { "ExclusiveWith" }, new object[] { new string[] { "a" } });
			ConfigTestHelper.AddVerb(_assembly, "Test2", "x", null, "b", "testb", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbExecutionOrderValid()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", null, null, new string[] { "ExecutionOrder" }, new object[] { (int?)1 }, null, null);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t2", "test2", null, null, new string[] { "ExecutionOrder" }, new object[] { (int?)2 }, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void VerbExecutionOrderNotValid()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", null, null, new string[] { "ExecutionOrder" }, new object[] { 2 }, null, null);
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t2", "test2", null, null, new string[] { "ExecutionOrder" }, new object[] { 2 }, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbAndOptionNamesUnique()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", "e1", "extra1");
			ConfigTestHelper.AddVerb(_assembly, "Test2", "t2", "test2", "e2", "extra2", null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void VerbAndOptionNamesNotUniqueDifferentVerbs()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", "e1", "extra1");
			ConfigTestHelper.AddVerb(_assembly, "Test2", "e1", "extra1", null, null, null, null, null, null, null, true);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbAndOptionNamesNotUniqueSameVerbs()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "e1", "test1", "e1", "extra1");

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbAndOptionNamesNotUniqueSameVerb()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t1", "test1", "t1", "test1");

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionDataTypeValidString1()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(string));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidString2()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(String));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidChar()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(char));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidBool()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(bool));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidInt16()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(Int16));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidInt32()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(Int32));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidInt64()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(Int64));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidShort()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(short));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidInt()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(int));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidLong()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(long));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidSingle()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(Single));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidFloat()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(float));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidDecimal()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(decimal));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidDouble()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(double));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidDateTime()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(DateTime));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeValidEnum()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", null, null, null, null, null, null, typeof(ConditionOperator));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void OptionDataTypeInvalid()
		{
			ConfigTestHelper.AddVerb(_assembly, "Test1", "t", "test", "o", "opt", null, null, null, null, typeof(Dictionary<int, string>));

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void DefaultHelpPresent()
		{
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test1", true, true, null, null, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void DefaultHelpMissing()
		{
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test1", false, true, null, null, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void DefaultHelpUsedMoreThanOnce()
		{
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test1", true, true, null, null, null, null);
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test2", true, false, null, null, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}

		[TestMethod]
		public void DefaultVersionPresent()
		{
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test1", true, true, null, null, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void DefaultVersionMissing()
		{
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test1", true, false, null, null, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
		}

		[TestMethod]
		public void DefaultVersionUsedMoreThanOnce()
		{
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test1", true, true, null, null, null, null);
			ConfigTestHelper.AddSingleVerbForDefaultVerbs(_assembly, "Test2", false, true, null, null, null, null);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}

		[TestMethod]
		public void DuplicateOptionsDefined()
		{
			var options = new List<ConfigTestOptionParameter>();

			var op1 = new ConfigTestOptionParameter()
			{
				Name = "prm1",
				AlternateNames = Array.Empty<string>()
			};

			var op2 = new ConfigTestOptionParameter()
			{
				Name = "prm1",
				AlternateNames = Array.Empty<string>()
			};

			options.Add(op1);
			options.Add(op2);

			ConfigTestHelper.AddSingleVerbWithMultipleOptions(_assembly, "Test1", "tc", "Test", null, null, options);

			_cm.Process(new string[] { "" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionAllowNullsNotNeededWithoutNeedsValue()
		{
			var options = new List<ConfigTestOptionParameter>();

			var op1 = new ConfigTestOptionParameter()
			{
				DataType = typeof(string),
				Name = "o",
				AlternateNames = new string[] { "option" },
				PropertyNames = new string[] { "AllowNullValue", "NeedsValue" },
				PropertyValues = new object[] { true, false }
			};

			options.Add(op1);

			ConfigTestHelper.AddSingleVerbWithMultipleOptions(_assembly, "Test1", "t", "test", null, null, options);

			_cm.Process(Array.Empty<string>(), _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbStartsWithPrefixName()
		{
			DefaultValues.VerbPrefixes.Add("x");

			ConfigTestHelper.AddVerb(_assembly, "Test", "x_verb", "test", null, null, null, null, null, null, null, true);

			_cm.Process(Array.Empty<string>(), _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void VerbStartsWithPrefixAlternateName()
		{
			var name = "*";
			DefaultValues.VerbPrefixes.Add("*");

			ConfigTestHelper.AddVerb(_assembly, "Test", "test", name, null, null, null, null, null, null, null, true);

			_cm.Process(Array.Empty<string>(), _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}

		[TestMethod]
		public void OptionStartsWithPrefixName()
		{
			const string name = "x";
			DefaultValues.OptionPrefixes.Add(name);
			var options = new List<ConfigTestOptionParameter>();

			var op1 = new ConfigTestOptionParameter()
			{
				DataType = typeof(string),
				Name = name,
				AlternateNames = new string[] { "option" },
				PropertyNames = Array.Empty<string>(),
				PropertyValues = Array.Empty<object>()
			};

			options.Add(op1);

			ConfigTestHelper.AddSingleVerbWithMultipleOptions(_assembly, "Test1", "t", "test", null, null, options);

			_cm.Process(Array.Empty<string>(), _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(1);
		}

		[TestMethod]
		public void OptionStartsWithPrefixAlternateName()
		{
			string name = "*";
			DefaultValues.OptionPrefixes.Add("*");
			var options = new List<ConfigTestOptionParameter>();

			var op1 = new ConfigTestOptionParameter()
			{
				DataType = typeof(string),
				Name = "o",
				AlternateNames = new string[] { name },
				PropertyNames = Array.Empty<string>(),
				PropertyValues = Array.Empty<object>()
			};

			options.Add(op1);

			ConfigTestHelper.AddSingleVerbWithMultipleOptions(_assembly, "Test1", "t", "test", null, null, options);

			_cm.Process(Array.Empty<string>(), _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

			_cm.Validity.ConfigurationErrorMessages.Should().HaveCount(2);
		}
	}
}