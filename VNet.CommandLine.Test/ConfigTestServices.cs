using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.IO;
using VNet.CommandLine.Validation;
using VNet.CommandLine.Validation.FluentValidation;
using VNet.Testing;
using VNet.Validation;

namespace VNet.CommandLine.Test
{
	public static class ConfigTestServices
    {
        public static TestServices ConfigureServices()
        {
            var additionalValidators = new List<IValidator>()
            {
                new ArgumentArrayValueValidator(),
                new ArgumentValuePairValidator(),
                new ArgumentStartValidator()
            };

            var alternateValidatorServices = new ServiceCollection();
            alternateValidatorServices.AddScoped<List<IValidator>>(p => 
                ActivatorUtilities.CreateInstance<List<IValidator>>(p, 
                    additionalValidators));

            var alternateValidatorServiceProvider = alternateValidatorServices.BuildServiceProvider();



            var validatorServices = new ServiceCollection();
            validatorServices.AddScoped<IValidator<ICommandLine>, CommandLineValidator>()
                .AddScoped(p => alternateValidatorServiceProvider.GetService<List<IValidator>>());
            
            var validatorServiceProvider = validatorServices.BuildServiceProvider();



            var validationParametersServices = new ServiceCollection();
            validationParametersServices.AddScoped<ICommandLineValidatorParameters>(p =>
                ActivatorUtilities.CreateInstance<DefaultCommandLineValidatorParameters>(p, 
                    new object[]
                    {
                        validatorServiceProvider.GetService<IValidator<ICommandLine>>(),
                        validatorServiceProvider.GetService<List<IValidator>>()
                    })); 

            var validationParametersServiceProvider = validationParametersServices.BuildServiceProvider();



            var assemblyServices = new ServiceCollection();
            assemblyServices.AddTransient<IAssembly, FakeAssemblyWrapper>();

            var assemblyServiceProvider = assemblyServices.BuildServiceProvider();



            var services = new ServiceCollection();
            services.AddSingleton<TextWriter>(t =>
                    ActivatorUtilities.CreateInstance<StreamWriter>(t, new MemoryStream()))
                .AddScoped<ILoader, DefaultLoader>()
                .AddScoped<IParser, DefaultParser>()
                .AddScoped<IConditionProcessor, DefaultConditionProcessor>()
                .AddScoped<ICommandLineValidatorManager>(m =>
                    ActivatorUtilities.CreateInstance<DefaultValidator>(m, 
                        // ReSharper disable once AssignNullToNotNullAttribute
                        validationParametersServiceProvider.GetService<ICommandLineValidatorParameters>()
                        ))
                .AddScoped<IDisplayer, DefaultDisplayer>()
                .AddScoped<IExecutor, DefaultExecutor>();

            var serviceProvider = services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance))
                                          .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Information)
                                          .BuildServiceProvider();



            var ts = new TestServices(validatorServiceProvider, validationParametersServiceProvider,
                assemblyServiceProvider, serviceProvider);
            
            return ts;
        }
    }
}