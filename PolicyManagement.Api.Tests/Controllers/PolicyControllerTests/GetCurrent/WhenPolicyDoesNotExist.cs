using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.GetCurrent
{
    [TestFixture]
    public class WhenPolicyDoesNotExist : PolicyControllerTestBase
    {
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            Service.Setup(s => s.GetCurrentAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as PolicyModel);

            _output = await ClassInTest.GetCurrentPolicy(MockCancellationToken);
        }

        [Test]
        public void NoContent_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(NoContentResult)));
        }

        [Test]
        public void Policy_Is_Requested()
        {
            Service.Verify(s => s.GetCurrentAsync(It.Is<CancellationToken>(x => x == MockCancellationToken)), Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
