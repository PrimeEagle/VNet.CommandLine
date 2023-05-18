using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace VNet.CommandLine.Test
{
    [ExcludeFromCodeCoverage]
    public class TestServices
    {
        public ServiceProvider AssemblyServiceProvider { get; }
        public ServiceProvider ValidatorServiceProvider { get; }
        public ServiceProvider ValidationParameterServiceProvider { get; }
        public ServiceProvider ApplicationServiceProvider { get; }

        public TestServices(ServiceProvider validator, ServiceProvider validationParameter,
            ServiceProvider assembly, ServiceProvider application)
        {
            this.AssemblyServiceProvider = assembly;
            this.ValidatorServiceProvider = validator;
            this.ValidationParameterServiceProvider = validationParameter;
            this.ApplicationServiceProvider = application;
        }
    }
}
