using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.GetDraftAsync
{
    [TestFixture]
    public class WhenPolicyExists : PolicyServiceTestBase
    {
        private MemoryStream _expectedStream;
        private PolicyModel _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();
            
            _fileShare.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _fileShare.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_expectedStream = new MemoryStream());

            _serializer.Setup(s => s.Deserialize<PolicyModel>(It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedModel);

            _output = await ClassInTest.GetDraftAsync(Token);
        }

        [Test]
        public void Policy_Existence_Checked()
        {
            _fileShare.Verify(s => s.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)));
        }

        [Test]
        public void Policy_Downloaded()
        {
            _fileShare.Verify(s => s.ReadAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)));
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

        [Test]
        public void Policy_Is_Returned()
        {
            Assert.That(_output, Is.EqualTo(ExpectedModel));
        }
    }
}
