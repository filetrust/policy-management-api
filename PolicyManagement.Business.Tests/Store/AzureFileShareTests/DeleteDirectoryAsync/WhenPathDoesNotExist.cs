using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Store.AzureFileShareTests.DeleteDirectoryAsync
{
    [TestFixture]
    public class WhenPathDoesNotExist : AzureFileShareTestBase
    {
        private CancellationToken _mockCancellationToken;
        private string _input;
        private Mock<ShareDirectoryClient> _directoryOfPathBeingDeleted;
        private Mock<Response<bool>> _existsResponse;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _directoryOfPathBeingDeleted = new Mock<ShareDirectoryClient>();
            
            ShareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
                .Returns(_directoryOfPathBeingDeleted.Object);

            _directoryOfPathBeingDeleted.Setup(s => s.ExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((_existsResponse = new Mock<Response<bool>>()).Object);

            _existsResponse.Setup(s => s.Value).Returns(false);

            await ClassInTest.DeleteDirectoryAsync(
                _input = "myPath",
                _mockCancellationToken = new CancellationToken(false));
        }

        [Test]
        public void Directory_Client_Is_Retrieved()
        {
            ShareClient.Verify(s => s.GetDirectoryClient(It.Is<string>(f => f == _input)), Times.Once);
        }

        [Test]
        public void Input_Path_Existence_Is_Checked()
        {
            _directoryOfPathBeingDeleted.Verify(s => s.ExistsAsync(It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }
    }
}
