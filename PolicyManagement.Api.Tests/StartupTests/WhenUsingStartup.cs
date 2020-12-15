using System;
using System.Configuration;
using System.Linq;
using Glasswall.PolicyManagement.Api;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Services;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Api.Tests.StartupTests
{
    [TestFixture]
    public class WhenUsingStartup : UnitTestBase<Startup>
    {
        [Test]
        public void Can_Resolve_Distributer_Service()
        {
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServiceEndpointCsv), "localhost");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServiceUsername), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServicePassword), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServiceUsername), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServicePassword), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv), "nameyname");

            ClassInTest = new Startup(new ConfigurationBuilder().AddEnvironmentVariables().Build());

            var services = new ServiceCollection();
            
            ClassInTest.ConfigureServices(services);
            
            Assert.That(services.Any(s =>
                s.ServiceType == typeof(IFileStore)), "No file store was added");

            services.BuildServiceProvider().GetRequiredService<IPolicyDistributer>();
        }

        [Test]
        public void Configuration_Can_Be_Parsed()
        {
            var services = new ServiceCollection();

            ClassInTest.ConfigureServices(services);

            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServiceEndpointCsv), "localhost");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServiceUsername), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServicePassword), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServiceUsername), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServicePassword), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv), "nameyname");

            ClassInTest = new Startup(new ConfigurationBuilder().AddEnvironmentVariables().Build());

            var config = services.BuildServiceProvider().GetRequiredService<IPolicyManagementApiConfiguration>();

            Assert.That(config.PolicyUpdateServiceEndpointCsv, Is.EqualTo("localhost"));
        }
        
        [Test]
        public void When_ConfigValue_Is_Missing()
        {
            var services = new ServiceCollection();

            ClassInTest = new Startup(new ConfigurationBuilder().Build());

            ClassInTest.ConfigureServices(services);

            Assert.That(() => services.BuildServiceProvider().GetRequiredService<IPolicyManagementApiConfiguration>(), Throws.Exception.InstanceOf<ConfigurationErrorsException>());
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Params()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<Startup>();
        }

        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<Startup>();
        }
    }
}
