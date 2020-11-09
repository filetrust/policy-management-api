using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.SavePolicy
{
    [TestFixture]
    public class WhenExecutingAction : PolicyControllerTestBase
    {
        private IActionResult _output;
        private PolicyModel _expectedPolicy;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            _expectedPolicy = new PolicyModel();

            _output = await ClassInTest.SavePolicy(_expectedPolicy, MockCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(OkResult)));
        }

        [Test]
        public void Service_Is_Called()
        {
            Service.Verify(
                s => s.SaveAsDraftAsync(
                    It.Is<PolicyModel>(f => f == _expectedPolicy),
                    It.Is<CancellationToken>(x => x == MockCancellationToken)),
                Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
