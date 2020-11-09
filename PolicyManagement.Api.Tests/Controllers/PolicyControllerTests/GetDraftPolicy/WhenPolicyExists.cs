using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.GetDraftPolicy
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

            Service.Setup(s => s.GetDraftAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_expectedPolicy);

            _output = await ClassInTest.GetDraftPolicy(MockCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(OkObjectResult)));
        }

        [Test]
        public void Body_Is_Policy()
        {
            var body = ((OkObjectResult) _output).Value as PolicyModel;

            Assert.That(body, Is.Not.Null);
            Assert.That(body, Is.EqualTo(_expectedPolicy));
        }

        [Test]
        public void Policy_Is_Requested()
        {
            Service.Verify(s => s.GetDraftAsync(It.Is<CancellationToken>(x => x == MockCancellationToken)), Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
