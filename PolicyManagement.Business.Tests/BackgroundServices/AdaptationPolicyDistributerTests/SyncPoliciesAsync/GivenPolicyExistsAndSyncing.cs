using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.BackgroundServices.AdaptationPolicyDistributerTests.SyncPoliciesAsync
{
    [TestFixture]
    public class GivenPolicyExistsAndSyncing : AdaptationPolicyDistributerTestBase
    {
        private PolicyModel _policyModel;
        private const string Endpoint1 = "http://endpoint1.com";
        private const string Endpoint2 = "http://endpoint2.com";

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            Configuration.SetupGet(s => s.PolicyUpdateServiceEndpointCsv)
                .Returns($"{Endpoint1},{Endpoint2}");

            Service.Setup(s => s.GetCurrentAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_policyModel = new PolicyModel());

            await ClassInTest.SyncPoliciesAsync(Token);
        }

        [Test]
        [Order(1)]
        public void Configuration_Is_Accessed_Correctly()
        {
            Configuration.VerifyGet(s => s.PolicyUpdateServiceEndpointCsv, Times.Exactly(3));
            Configuration.VerifyNoOtherCalls();
        }

        [Test]
        [Order(2)]
        public void Current_Policy_Is_Retrieved_The_First_Time()
        {
            Service.Verify(s => s.GetCurrentAsync(It.Is<CancellationToken>(x => x == Token)), Times.Once);
            Service.VerifyNoOtherCalls();
        }

        [Test]
        [Order(3)]
        public void Current_Policy_Is_Distributed_The_First_Time()
        {
            Distributer.Verify(s => s.DistributeAdaptionPolicy(It.Is<PolicyModel>(f => f == _policyModel), It.Is<CancellationToken>(x => x == Token)), Times.Once);
            Distributer.VerifyNoOtherCalls();
        }

        [Test]
        [Order(4)]
        public async Task Subsequent_Syncs_Do_No_Distribute()
        {
            for (var i = 0; i < 100; i++)
            {
                Configuration.Invocations.Clear();
                Service.Invocations.Clear();
                Distributer.Invocations.Clear();

                await ClassInTest.SyncPoliciesAsync(Token);
                
                Configuration.VerifyGet(s => s.PolicyUpdateServiceEndpointCsv, Times.Once);
                Configuration.VerifyNoOtherCalls();
                Service.VerifyNoOtherCalls();
                Distributer.VerifyNoOtherCalls();
            }
        }
    }
}
