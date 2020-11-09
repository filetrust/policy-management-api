using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.PublishDraft
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

            _output = await ClassInTest.PublishDraft(_input = Guid.NewGuid(), MockCancellationToken);
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
                s => s.PublishAsync(
                    It.Is<Guid>(f => f == _input),
                    It.Is<CancellationToken>(x => x == MockCancellationToken)),
                Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
