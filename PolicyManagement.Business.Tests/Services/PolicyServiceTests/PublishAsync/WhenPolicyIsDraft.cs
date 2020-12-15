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
    public class WhenPolicyIsDraft : PolicyServiceTestBase
    {
        private Guid _input;
        private PolicyModel _currentPolicy;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            ExpectedModel.Id = _input = Guid.NewGuid();
            ExpectedModel.PolicyType = PolicyType.Draft;

            _currentPolicy = new PolicyModel
            {
                Id = Guid.NewGuid(),
                PolicyType = PolicyType.Current
            };

            _fileShare.Setup(s => s.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _fileShare.Setup(s => s.ExistsAsync(It.Is<string>(f => f == "current/policy.json"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _fileShare.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());

            _serializer.Setup(s => s.Serialize(It.IsAny<PolicyModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("json");

            var downloadSequence = _serializer.SetupSequence(s => s.Deserialize<PolicyModel>(It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()));

            downloadSequence.ReturnsAsync(_currentPolicy);
            downloadSequence.ReturnsAsync(ExpectedModel);
            downloadSequence.ReturnsAsync(_currentPolicy);

            await ClassInTest.PublishAsync(_input, Token);
        }

        [Test]
        [Order(1)]
        public void Each_Policy_Is_Checked_For_Existence()
        {
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == $"historical/{_input}/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == "current/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Exactly(2));
            _fileShare.Verify(x => x.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        [Order(2)]
        public void Current_Is_Downloaded()
        {
            _fileShare.Verify(x => x.ReadAsync(It.Is<string>(f => f == "current/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Exactly(2));
        }

        [Test]
        [Order(3)]
        public void Draft_Is_Downloaded()
        {
            _fileShare.Verify(x => x.ReadAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        [Order(4)]
        public void InputPolicy_Is_Published()
        {
            _fileShare.Verify(x => x.WriteAsync(It.Is<string>(f => f == "current/policy.json"), It.IsAny<byte[]>(), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        [Order(5)]
        public void Current_Is_Moved_To_Historic()
        {
            _fileShare.Verify(x => x.WriteAsync(It.Is<string>(f => f == $"historical/{_currentPolicy.Id}/policy.json"), It.IsAny<byte[]>(), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        [Order(6)]
        public void Draft_Is_Deleted()
        {
            _fileShare.Verify(x => x.DeleteAsync(It.Is<string>(f => f == "draft"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        [Order(7)]
        public void No_Other_Operations_Called()
        {
            _fileShare.VerifyNoOtherCalls();
        }
    }
}