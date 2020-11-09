using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.GetPolicy
{
    [TestFixture]
    public class WhenPolicyDoesNotExist : PolicyControllerTestBase
    {
        private IActionResult _output;
        private Guid _input;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            Service.Setup(s => s.GetPolicyByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as PolicyModel);

            _output = await ClassInTest.GetPolicy(_input = Guid.NewGuid(), MockCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(NoContentResult)));
        }

        [Test]
        public void Service_Is_Called()
        {
            Service.Verify(
                s => s.GetPolicyByIdAsync(
                    It.Is<Guid>(f => f == _input),
                    It.Is<CancellationToken>(x => x == MockCancellationToken)),
                Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
