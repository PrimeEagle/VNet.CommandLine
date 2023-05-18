using System;
using System.Collections.Generic;

namespace VNet.CommandLine
{
	public interface ICategory : IBaseCommand
    {
		List<IVerb> Verbs { get; set; }
	    List<IOption> Options { get; set; }
	}
}
