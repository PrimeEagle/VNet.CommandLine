using System.Collections.Generic;
using System.IO;
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
    public class OptionValidationTests
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
        public void DuplicateOptionAllowDuplicatesFalse()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s s -i d r" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void DuplicateOptionAllowDuplicatesTrue()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -i d d r" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void RequiredOptionPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c -m t -i" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void RequiredOptionMissing()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c -m -i" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void DependencyPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c -m t -i -sv so1 so2" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void DependencyMissing()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c -m t -i -sv so1" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void ExclusivityMissing()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c -m t -i -sv so1 so2" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ExclusivityPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c -m t -i -sv so1 so2 so3" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void NeedsValueAndHasOneWithSeparator()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so4:test" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void NeedsValueAndHasOneWithoutSeparator()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so4 test" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void NeedsValueAndHasNone()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so4" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void DoesNotNeedValueHasOneWithSeparator()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so5:test" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void AllowsNullValueNull()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so6:" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void AllowsNullValueNotNull()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so6:test" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void DoesNotAllowsNullValueNull()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so4:" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void DoesNotAllowsNullValueNotNull()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so4:test" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsString1()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv string1:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsString2()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv string2:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsChar()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv char:t" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotChar()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv char:test" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsBool()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv bool:true" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotBool()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv bool:test" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsDateTime()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv datetime:2021-05-31" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotDateTime()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv datetime:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsInt()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int:77" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotInt()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsShort()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv short:54" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotShort()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv short:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsLong()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv log:87123" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotLong()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv long:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsInt16()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int16:31" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotInt16()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int16:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsInt32()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int32:8122" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotInt32()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int32:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsInt64()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int64:90182" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotInt64()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv int64:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsDouble()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv double:90.012" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotDouble()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv double:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsSingle()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv single:54.12" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotSingle()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv single:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsDecimal()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv decimal:9.0182" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotDecimal()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv decimal:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsFloat()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv float:15.415" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotFloat()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv float:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void ValueIsCustomEnum()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv enum:Or" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotEnum()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv enum:testing" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(2);
        }

        [TestMethod]
        public void OptionHasDuplicateAndValueWithSeparator()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so7:testing1 so7:testing2" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void OptionHasDuplicateAndValueWithoutSeparator()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv so7 testing1 so7 testing2" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsArray()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv array:[a, b, c]" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ValueIsNotArray()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c i -sv array:123" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(5);
        }
    }
}