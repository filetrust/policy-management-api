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
    public class WhenPolicyExists : PolicyControllerTestBase
    {
        private IActionResult _output;
        private Guid _input;
        private PolicyModel _expectedPolicy;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            Service.Setup(s => s.GetPolicyByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_expectedPolicy = new PolicyModel());

            _output = await ClassInTest.GetPolicy(_input = Guid.NewGuid(), MockCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)_output).Value, Is.EqualTo(_expectedPolicy));
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
