using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using VNet.Validation;

namespace VNet.CommandLine
{
    public interface IDisplayer
    {
	    void ShowOutput(IEnumerable<TextWriter> streams,
		    ILogger logger, IConfiguration configuration,
		    ValidationState validity,
		    ProcessingOptions processingOptions);
        void ShowHelp(IEnumerable<TextWriter> textWriters, ILogger logger, IConfiguration configuration);
        void ShowErrors(IEnumerable<TextWriter> textWriters, ILogger logger, IEnumerable<string> errorMessages, bool log);
    }
}
