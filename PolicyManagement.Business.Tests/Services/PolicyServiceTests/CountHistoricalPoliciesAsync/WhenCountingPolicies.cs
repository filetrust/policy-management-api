using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Business.Store;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Store;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.CountHistoricalPoliciesAsync
{
    [TestFixture]
    public class WhenCountingPolicies : PolicyServiceTestBase
    {
        private int _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _fileShare.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<IPathActions>(), It.IsAny<CancellationToken>()))
                .Returns(new [] { "policy1", "policy2", "policy3" }.AsAsyncEnumerable());

            _output = await ClassInTest.CountHistoricalPoliciesAsync(Token);
        }

        [Test]
        public void Policy_Search_Executed()
        {
            _fileShare.Verify(s => s.SearchAsync("", It.IsAny<HistorySearch>(), It.Is<CancellationToken>(f => f == Token)), Times.Once);
            _fileShare.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Policy_Count_Is_Returned()
        {
            Assert.That(_output, Is.EqualTo(3));
        }
    }
}
