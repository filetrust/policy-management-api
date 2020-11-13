using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Files.Shares;
using Glasswall.PolicyManagement.Api;
using Glasswall.PolicyManagement.Business.Configuration;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Services;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Api.Tests.StartupTests
{
    [TestFixture]
    public class WhenUsingStartup : UnitTestBase<Startup>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ClassInTest = new Startup(Mock.Of<IConfiguration>());
        }

        [Test]
        public void Can_Resolve_Distributer_Service()
        {
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServiceEndpointCsv), "localhost");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.ShareName), "policies");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.AccountKey), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.AccountName), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.TokenUsername), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.TokenPassword), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv), "nameyname");

            var services = new ServiceCollection();
            
            ClassInTest.ConfigureServices(services);
            
            Assert.That(services.Any(s =>
                s.ServiceType == typeof(ShareClient)), "No share client was added");

            Assert.That(services.Any(s =>
                s.ServiceType == typeof(IFileShare)), "No file store was added");

            services.Replace(new ServiceDescriptor(typeof(IEnumerable<ShareClient>),
                new [] { Mock.Of<ShareClient>() }));

            services.BuildServiceProvider().GetRequiredService<IPolicyDistributer>();
        }

        [Test]
        public void Configuration_Can_Be_Parsed()
        {
            var services = new ServiceCollection();

            ClassInTest.ConfigureServices(services);

            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.PolicyUpdateServiceEndpointCsv), "localhost");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.ShareName), "policies");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.AccountKey), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.AccountName), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.TokenUsername), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.TokenPassword), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv), "nameyname");

            var config = services.BuildServiceProvider().GetRequiredService<IPolicyManagementApiConfiguration>();

            Assert.That(config.PolicyUpdateServiceEndpointCsv, Is.EqualTo("localhost"));
            Assert.That(config.ShareName, Is.EqualTo("policies"));
            Assert.That(config.AccountKey, Is.EqualTo("keymckey"));
            Assert.That(config.AccountName, Is.EqualTo("nameyname"));
        }
        
        [Test]
        public void When_ConfigValue_Is_Missing()
        {
            var services = new ServiceCollection();

            Environment.SetEnvironmentVariable(nameof(IPolicyManagementApiConfiguration.ShareName), "");

            ClassInTest.ConfigureServices(services);

            Assert.That(() => services.BuildServiceProvider().GetRequiredService<IPolicyManagementApiConfiguration>(), Throws.Exception.InstanceOf<ConfigurationBindException>());
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
