﻿using System.Collections.Generic;
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
    public class VerbValidationTests
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
        public void UnknownVerbNotUsed()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -i d" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void UnknownVerbUsed()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -i d -xyz" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void RequiredVerbPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -i d" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void RequiredVerbMissing()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-m t -i d" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void DuplicateVerbsNotPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -m t -i d" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void DuplicateVerbsPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -m t -c -i" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void VerbDependencyPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -m t -i" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void VerbDependencyNotPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -m t" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }


        [TestMethod]
        public void VerbExclusivityPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -m t -i -e" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().HaveCount(1);
        }

        [TestMethod]
        public void VerbExclusivityNotPresent()
        {
            ConfigTestHelper.CreateValidConfiguration(_assembly);

            _cm.Process(new string[] { "-c s -m t -i" }, _assembly, _streams, _logger, _config, _parse, _conditionProcessor, _validate, _display, _executor);

            _cm.Validity.ConfigurationErrorMessages.Should().BeEmpty();
            _cm.Validity.UsageErrorMessages.Should().BeEmpty();
        }
    }
}