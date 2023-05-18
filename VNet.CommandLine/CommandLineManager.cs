using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using VNet.CommandLine.Validation;
using VNet.Testing;
using VNet.Validation;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable BuiltInTypeReferenceStyle

namespace VNet.CommandLine
{
    public class CommandLineManager
	{
		public ValidationState Validity { get; init; }
		public ProcessingOptions ProcessingOptions { get; init; }

		public CommandLineManager()
		{
			this.ProcessingOptions = new ProcessingOptions();
			this.Validity = new ValidationState();

			InitializeDefaults();
		}

		private void InitializeDefaults()
		{
			this.ProcessingOptions.LogConfigurationErrors = true;
			this.ProcessingOptions.LogUsageErrors = false;
			this.ProcessingOptions.DisplayHelpOnConfigurationErrors = false;
			this.ProcessingOptions.DisplayHelpOnUsageErrors = true;
			this.ProcessingOptions.DisplayConfigurationErrorMessages = true;
			this.ProcessingOptions.DisplayUsageErrorMessages = true;
			this.ProcessingOptions.StopIfConfigurationErrors = true;

			DefaultValues.HelpName = "h";
			DefaultValues.HelpAlternateNames = new List<string> { "help", "?" };
			DefaultValues.HelpHelpText = "Displays usage information";
			DefaultValues.VersionName = "v";
			DefaultValues.VersionAlternateNames = new List<string> { "version" };
			DefaultValues.VersionHelpText = "Displays version information";
			DefaultValues.CategoryPrefixes = new List<string> { "", "-", "--" };
			DefaultValues.VerbPrefixes = new List<string> { "", "-", "--", "/", "\\" };
			DefaultValues.OptionPrefixes = new List<string> { "", "-", "--" };
			DefaultValues.OptionValueSeparators = new List<string> { "=", ":" };
			DefaultValues.OptionValueArrayStartDelimiters = new List<string> { "'", "\"", "[" };
			DefaultValues.OptionValueArrayEndDelimiters = new List<string> { "'", "\"", "]" };
			DefaultValues.OptionValueArrayDelimiters = new List<string> { ",", ";", "\t" };
			DefaultValues.AllowedDataTypes = new List<Type>{
				typeof(string), typeof(String), typeof(char), typeof(bool),
				typeof(Int16), typeof(Int32), typeof(Int64),
				typeof(short), typeof(int), typeof(long),
				typeof(Single), typeof(float), typeof(decimal), typeof(double),
				typeof(DateTime), typeof(Enum), typeof(Array)
			};
		}

		public void Process(string[] args, IAssembly assembly, IEnumerable<TextWriter> streams,
							ILogger logger, ILoader loader, IParser parser, IConditionProcessor conditionProcessor,
                            ICommandLineValidatorManager validator, IDisplayer display, IExecutor executor)
		{
			var configuration = loader.Load(new object[] { assembly });
			var usage = parser.Parse(args, configuration, validator);
            usage = conditionProcessor.Process(configuration, usage);
			
            var commandLine = new CommandLine(configuration, usage);
			var validity = validator.Validate(commandLine, usage.Validity);

			this.Validity.Append(validity);

			if (this.Validity.Valid)
			{
				executor.Execute(usage);
			}
			else
			{
				display.ShowOutput(streams, logger, configuration, this.Validity, this.ProcessingOptions);
			}
		}
	}
}