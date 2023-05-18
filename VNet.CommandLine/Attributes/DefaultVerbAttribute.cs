using System;
using System.Diagnostics.CodeAnalysis;

namespace VNet.CommandLine.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    [ExcludeFromCodeCoverage]
    public class DefaultVerbAttribute : Attribute, IAttribute
    {

    }
}