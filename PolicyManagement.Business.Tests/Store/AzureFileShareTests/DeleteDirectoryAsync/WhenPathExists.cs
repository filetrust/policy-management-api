using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Store.AzureFileShareTests.DeleteDirectoryAsync
{
    [TestFixture]
    public class WhenPathExists : AzureFileShareTestBase
    {
        private CancellationToken _mockCancellationToken;
        private string _input;
        private Mock<ShareDirectoryClient> _directoryOfPathBeingDeleted;
        private ShareFileItem _subFolder;
        private ShareFileItem _subFile;
        private Mock<Response<bool>> _existsResponse;
        private Mock<ShareDirectoryClient> _subDirectoryClient;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _directoryOfPathBeingDeleted = new Mock<ShareDirectoryClient>();
            _subFolder = FilesModelFactory.StorageFileItem(true, "folder", 0);
            _subFile = FilesModelFactory.StorageFileItem(false, "file", 0);
            
            ShareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
                .Returns(_directoryOfPathBeingDeleted.Object);

            var subContents = new[] {_subFolder, _subFile};
            
            _directoryOfPathBeingDeleted.Setup(s => s.ExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((_existsResponse = new Mock<Response<bool>>()).Object);

            _directoryOfPathBeingDeleted.Setup(x => x.GetFilesAndDirectoriesAsync(null, It.IsAny<CancellationToken>()))
                .Returns(MockPageable(subContents).Object);

            _directoryOfPathBeingDeleted.Setup(s => s.GetSubdirectoryClient(It.IsAny<string>()))
                .Returns((_subDirectoryClient = new Mock<ShareDirectoryClient>()).Object);

            _subDirectoryClient.Setup(x => x.GetFilesAndDirectoriesAsync(null, It.IsAny<CancellationToken>()))
                .Returns(MockPageable(Enumerable.Empty<ShareFileItem>()).Object);

            _existsResponse.Setup(s => s.Value).Returns(true);

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

        [Test]
        public void SubDirectory_Is_Deleted()
        {
            _subDirectoryClient.Verify(x => x.DeleteIfExistsAsync(It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }

        [Test]
        public void SubDirectory_Is_Listed()
        {
            _subDirectoryClient.Verify(x => x.GetFilesAndDirectoriesAsync(null, It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }

        [Test]
        public void Subfile_Is_Deleted()
        {
            _directoryOfPathBeingDeleted.Verify(x => x.DeleteFileAsync(It.Is<string>(f => f == "file"), It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }

        [Test]
        public void Input_Directory_Is_Deleted()
        {
            _directoryOfPathBeingDeleted.Verify(x => x.DeleteIfExistsAsync(It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }
    }
}
