using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.PublishAsync
{
    [TestFixture]
    public class WhenPolicyIsCurrent : PolicyServiceTestBase
    {
        private Guid _input;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _fileShare.Setup(s => s.ExistsAsync(It.Is<string>(f => f == "current/policy.json"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _fileShare.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());

            _serializer.Setup(s => s.Deserialize<PolicyModel>(It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedModel);

            ExpectedModel.Id = _input = Guid.NewGuid();
            ExpectedModel.PolicyType = PolicyType.Current;

            await ClassInTest.PublishAsync(_input, Token);
        }

        [Test]
        [Order(1)]
        public void Each_Policy_Is_Checked_For_Existence()
        {
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == $"historical/{_input}/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == "current/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Never);
        }

        [Test]
        [Order(2)]
        public void Current_Is_Downloaded()
        {
            _fileShare.Verify(x => x.ReadAsync(It.Is<string>(f => f == "current/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        [Order(3)]
        public void No_Other_Operations_Called()
        {
            _fileShare.VerifyNoOtherCalls();
        }
    }
}