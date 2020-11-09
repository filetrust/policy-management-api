using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.GetHistoric
{
    [TestFixture]
    public class WhenPolicyDoesNotExist : PolicyControllerTestBase
    {
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            Service.Setup(s => s.GetHistoricalPoliciesAsync(It.IsAny<CancellationToken>()))
                .Returns(Enumerable.Empty<PolicyModel>().AsAsyncEnumerable());

            _output = await ClassInTest.GetHistoricPolicies(MockCancellationToken);
        }

        [Test]
        public void NoContent_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(NoContentResult)));
        }

        [Test]
        public void Policy_Is_Requested()
        {
            Service.Verify(s => s.GetHistoricalPoliciesAsync(It.Is<CancellationToken>(x => x == MockCancellationToken)), Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
