using Glasswall.PolicyManagement.Business.Services;
using Glasswall.PolicyManagement.Common.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using TestCommon;

namespace PolicyManagement.Business.Tests.Services.PolicyDistributerTests.DistributeAdaptionPolicy
{
    [TestFixture]
    public class WhenPolicyIsNull : UnitTestBase<PolicyDistributer>
    {
        private Mock<ILogger<PolicyDistributer>> _logger;
        private Mock<IPolicyManagementApiConfiguration> _configuration;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<PolicyDistributer>>();
            _configuration = new Mock<IPolicyManagementApiConfiguration>();

            ClassInTest = new PolicyDistributer(_logger.Object, _configuration.Object);

            _configuration.Setup(s => s.PolicyUpdateServiceEndpointCsv)
                .Returns("https://endpoint1:3001,http://endpoint2:401");
        }

        [Test]
        public void Exception_Raised()
        {
            Assert.That(async () =>
            await ClassInTest.DistributeAdaptionPolicy(null, new CancellationToken()), ThrowsArgumentNullException("policy"));
        }
    }
}
