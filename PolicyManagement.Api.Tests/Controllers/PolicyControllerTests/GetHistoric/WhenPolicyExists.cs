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
    public class WhenPolicyExists : PolicyControllerTestBase
    {
        private IActionResult _output;
        private PolicyModel _expectedPolicy;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            _expectedPolicy = new PolicyModel();

            Service.Setup(s => s.GetHistoricalPoliciesAsync(It.IsAny<CancellationToken>()))
                .Returns(new [] { _expectedPolicy }.AsAsyncEnumerable());

            _output = await ClassInTest.GetHistoricPolicies(MockCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf(typeof(OkObjectResult)));
        }

        [Test]
        public void Body_Is_Policy()
        {
            var body = ((OkObjectResult) _output).Value as HistoryResponse;

            Assert.That(body, Is.Not.Null);
            Assert.That(body.Policies.First(), Is.EqualTo(_expectedPolicy));
            Assert.That(body.PoliciesCount, Is.EqualTo(1));
        }

        [Test]
        public void Policy_Is_Requested()
        {
            Service.Verify(s => s.GetHistoricalPoliciesAsync(It.Is<CancellationToken>(x => x == MockCancellationToken)), Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
