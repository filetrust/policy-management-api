using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Glasswall.PolicyManagement.Api.Controllers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using PolicyManagement.Api.Function;

[assembly: FunctionsStartup(typeof(Startup))]

namespace PolicyManagement.Api.Function
{
    public class Startup : FunctionsStartup
    {
        [ExcludeFromCodeCoverage]
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var mainStartup = new Glasswall.PolicyManagement.Api.Startup(new ConfigurationRoot(new List<IConfigurationProvider>() { new EnvironmentVariablesConfigurationProvider()}));

            mainStartup.ConfigureServices(builder.Services);
            builder.Services.AddTransient<PolicyController>();
        }
    }
}