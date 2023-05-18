using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VNet.CommandLine.Validation;
using VNet.Testing;
using VNet.Validation;
// ReSharper disable NotAccessedField.Local

namespace VNet.CommandLine.Test.Tests
{
	[TestClass]
    public class CategoryTests
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
       public void Clone()
       {
	       var category = new Category("category");
	       var verb = new Verb("verb");
	       var option = new Option("option");

	       category.Verbs.Add(verb);
           category.Options.Add(option);

	       var clone = (Category)category.Clone();

	       clone.Name.Should().Be("category");
	       clone.Verbs.First().Name.Should().Be("verb");
	       clone.Options.First().Name.Should().Be("option");
       }
    }
}