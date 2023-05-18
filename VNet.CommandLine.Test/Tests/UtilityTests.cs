using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using VNet.CommandLine.Validation;
using VNet.Testing;
using VNet.Utility.Extensions;
using VNet.Validation;
// ReSharper disable NotAccessedField.Local

namespace VNet.CommandLine.Test.Tests
{
	[TestClass]
    public class UtilityTests
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Test ClassSetup")]
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
       public void GetAfterMatch()
       {
           const string s = "test:value";

           s.After(":").Should().Be("value");
       }

        [TestMethod]
        public void GetAfterNoMatch()
        {
            const string s = "test";

            s.After(":").Should().Be("test");
        }

        [TestMethod]
        public void GetBeforeMatch()
        {
            const string s = "test:value";

            s.Before(":").Should().Be("test");
        }

        [TestMethod]
        public void GetBeforeNoMatch()
        {
            const string s = "test";

            s.Before(":").Should().Be("test");
        }
    }
}