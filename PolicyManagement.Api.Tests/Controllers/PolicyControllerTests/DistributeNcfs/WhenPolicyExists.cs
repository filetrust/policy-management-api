using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.DistributeNcfs
{
    [TestFixture]
    public class WhenPolicyExists : PolicyControllerTestBase
    {
        private IActionResult _output;
        private PolicyModel _expectedPolicy;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            _expectedPolicy = new PolicyModel();

            Service.Setup(s => s.GetCurrentAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_expectedPolicy);

            _output = await ClassInTest.DistributeNcfsPolicy(MockCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(OkResult)));
        }
        
        [Test]
        public void Policy_Is_Requested()
        {
            Service.Verify(s => s.GetCurrentAsync(It.Is<CancellationToken>(x => x == MockCancellationToken)), Times.Once);
            Service.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Policy_Is_Distributed()
        {
            Distributer.Verify(x => x.DistributeNcfsPolicy(It.Is<PolicyModel>(f => f == _expectedPolicy), It.Is<CancellationToken>(f => f == MockCancellationToken)));
            Distributer.VerifyNoOtherCalls();
        }
    }
}
