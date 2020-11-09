using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.PublishAsync
{
    [TestFixture]
    public class WhenPolicyDoesNotExist : PolicyServiceTestBase
    {
        private Guid _input;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            await ClassInTest.PublishAsync(_input = Guid.NewGuid(), Token);
        }

        [Test]
        public void Historic_Draft_Current_Are_Retrieved()
        {
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)));
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == "current/policy.json"), It.Is<CancellationToken>(f => f == Token)));
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == $"historical/{_input}/policy.json"), It.Is<CancellationToken>(f => f == Token)));
            _fileShare.VerifyNoOtherCalls();
        }
    }
}