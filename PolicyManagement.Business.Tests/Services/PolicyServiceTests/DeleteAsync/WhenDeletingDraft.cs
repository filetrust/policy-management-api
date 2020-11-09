using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.DeleteAsync
{
    [TestFixture]
    public class WhenDeletingDraft : PolicyServiceTestBase
    {
        private Guid _input;
        private MemoryStream _expectedStream;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _fileShare.Setup(s => s.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _fileShare.Setup(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_expectedStream = new MemoryStream());

            _serializer.Setup(s => s.Deserialize<PolicyModel>(It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedModel);

            ExpectedModel.Id = Guid.NewGuid();

            await ClassInTest.DeleteAsync(_input = ExpectedModel.Id, Token);
        }

        [Test]
        public void Policy_Existence_Checked()
        {
            _fileShare.Verify(s => s.ExistsAsync(It.Is<string>(f => f == $"historical/{_input}"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        public void Policy_Is_Deleted()
        {
            _fileShare.Verify(s => s.DeleteDirectoryAsync(It.Is<string>(f => f == "draft"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        public void Draft_Policy_Existence_Checked()
        {
            _fileShare.Verify(s => s.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)));
        }

        [Test]
        public void Policy_Downloaded()
        {
            _fileShare.Verify(s => s.DownloadAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)));
        }

        [Test]
        public void Policy_Deserialised()
        {
            _serializer.Verify(s => s.Deserialize<PolicyModel>(
                    It.Is<MemoryStream>(f => f == _expectedStream),
                    It.Is<CancellationToken>(f => f == Token)),
                Times.Once);

            _serializer.VerifyNoOtherCalls();
        }
    }
}
