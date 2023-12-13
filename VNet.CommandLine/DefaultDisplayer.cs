using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using VNet.Validation;

// ReSharper disable PossibleMultipleEnumeration

namespace VNet.CommandLine
{
    public class DefaultDisplayer : IDisplayer
    {
	    public void ShowOutput(IEnumerable<TextWriter> streams,
            ILogger logger, IConfiguration configuration,
            ValidationState validity,
		    ProcessingOptions processingOptions)
	    {
		    if (validity.HasConfigurationErrors)
		    {
			    if (processingOptions.DisplayConfigurationErrorMessages)
				    ShowErrors(streams, logger, validity.ConfigurationErrorMessages, processingOptions.LogConfigurationErrors);

			    if (processingOptions.DisplayHelpOnConfigurationErrors)
				    ShowHelp(streams, logger, configuration);
		    }

		    if (validity.HasConfigurationErrors && processingOptions.StopIfConfigurationErrors) return;

		    if (processingOptions.DisplayUsageErrorMessages)
			    ShowErrors(streams, logger, validity.UsageErrorMessages, processingOptions.LogUsageErrors);

		    if (processingOptions.DisplayHelpOnUsageErrors)
			    ShowHelp(streams, logger, configuration);
        }

        public void ShowHelp(IEnumerable<TextWriter> streams, ILogger logger, IConfiguration configuration)
        {
            var processModule = global::System.Diagnostics.Process.GetCurrentProcess().MainModule;
            var textWriters = streams?.ToList();

            if (processModule != null)
            {
                var exeName = Path.GetFileName(processModule.FileName);

                WriteLineStreams(textWriters);
                WriteLineStreams(textWriters, $"Usage: {exeName} [commands] [options]");
            }

            foreach (var c in configuration.Categories)
            {
	            var categoryName = c.Name;
	            var categoryDescription = c.Help.Description;

	            WriteLineStreams(textWriters, $"  {categoryName} ({categoryDescription})");

                foreach (var v in c.Verbs.OrderForDisplay())
	            {
		            var verbAlternateName = string.Join(' ', v.AlternateNames);
		            var verbRequiredStart = v.Required ? string.Empty : "[";
		            var verbRequiredEnd = v.Required ? string.Empty : "]";

		            WriteLineStreams(textWriters, $"     {verbRequiredStart}{v.Name}{verbRequiredEnd} ({verbRequiredStart}{verbAlternateName}{verbRequiredEnd}          {v.Help.Description}");

                    foreach (var o in c.Options)
		            {
			            var optionAlternateName = string.Join(' ', o.AlternateNames);
			            var optionRequiredStart = o.Required ? string.Empty : "[";
			            var optionRequiredEnd = o.Required ? string.Empty : "]";

			            WriteLineStreams(textWriters, $"          {optionRequiredStart}{o.Name}{optionRequiredEnd} ({optionRequiredStart}{optionAlternateName}{optionRequiredEnd}          {o.Help.Description}");
                    }
                }
            }
        }

        public void ShowErrors(IEnumerable<TextWriter> streams, ILogger logger, IEnumerable<string> errorMessages, bool log)
        {
            var textWriters = streams?.ToList();

            foreach (var msg in errorMessages.Select(e => $"{e}"))
            {
                WriteLineStreams(textWriters, msg);
                if(log) WriteLogError(logger, msg);
            }
        }

        private static void WriteLogError(ILogger logger, string text = "")
        {
            logger?.LogError(text);
        }

        private static void WriteLineStreams(IEnumerable<TextWriter> streams, string text = "")
        {
            if (streams == null) return;

            foreach (var s in streams)
            {
                Console.SetOut(s);
                Console.WriteLine(text);
            }
        }
    }
}