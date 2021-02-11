using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.PostGetHistoric
{
    [TestFixture]
    public class WhenPolicyExists : PolicyControllerTestBase
    {
        private IActionResult _output;
        private PolicyModel _expectedPolicy;
        private GetHistoricPoliciesRequestModel _input;
        private PolicyModel[] _policies;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.SharedSetup();

            _expectedPolicy = new PolicyModel();

            _policies = Enumerable.Repeat(1, 50).Select(s => new PolicyModel
            {
                Id = Guid.NewGuid()
            }).ToArray();

            Service.Setup(s => s.GetHistoricalPoliciesAsync(It.IsAny<CancellationToken>()))
                .Returns(_policies.AsAsyncEnumerable());

            _output = await ClassInTest.GetHistoricPolicies(_input = new GetHistoricPoliciesRequestModel
            {
                Pagination = new PaginationModel
                {
                    PageSize = 25,
                    ZeroBasedIndex = 1
                }
            }, MockCancellationToken);
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
            Assert.That(body.Policies.First().Id, Is.EqualTo(_policies.ElementAt(25).Id));
            Assert.That(body.PoliciesCount, Is.EqualTo(25));
        }

        [Test]
        public void Policy_Is_Requested()
        {
            Service.Verify(s => s.GetHistoricalPoliciesAsync(It.Is<CancellationToken>(x => x == MockCancellationToken)), Times.Once);
            Service.Verify(s => s.CountHistoricalPoliciesAsync(It.Is<CancellationToken>(x => x == MockCancellationToken)), Times.Once);
            Service.VerifyNoOtherCalls();
        }
    }
}
