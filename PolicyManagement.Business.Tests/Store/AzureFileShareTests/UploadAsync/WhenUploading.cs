using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Store.AzureFileShareTests.UploadAsync
{
    [TestFixture]
    public class WhenUploading : AzureFileShareTestBase
    {
        private CancellationToken _mockCancellationToken;
        private Mock<ShareDirectoryClient> _mockDirectoryClient;
        private Mock<ShareFileClient> _mockFileClient;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            ShareClient.Setup(s => s.GetRootDirectoryClient())
                .Returns((_mockDirectoryClient = new Mock<ShareDirectoryClient>()).Object);

            var sequence = _mockDirectoryClient.SetupSequence(s => s.GetSubdirectoryClient(It.IsAny<string>()));

            sequence.Returns(_mockDirectoryClient.Object); // part1
            sequence.Returns(_mockDirectoryClient.Object); // part2
            sequence.Returns(_mockDirectoryClient.Object); // part3

            _mockDirectoryClient.Setup(x => x.GetFileClient(It.IsAny<string>()))
                .Returns((_mockFileClient = new Mock<ShareFileClient>()).Object);

            await ClassInTest.UploadAsync("part1/part2/part3/file.txt", Encoding.UTF8.GetBytes("Hello"),
                _mockCancellationToken = new CancellationToken(false));
        }
        
        [Test]
        public void Root_Directory_Is_Retrieved()
        {
            ShareClient.Verify(x => x.GetRootDirectoryClient(), Times.Once);
            ShareClient.VerifyNoOtherCalls();
        }

        [Test]
        public void Each_Directory_Part_Is_Created()
        {
            _mockDirectoryClient.Verify(
                s => s.CreateIfNotExistsAsync(null, null, null,
                    It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Exactly(3));
        }

        [Test]
        public void Each_Direct_Part_Is_Retrieved()
        {
            _mockDirectoryClient.Verify(
                s => s.GetSubdirectoryClient(It.Is<string>(f => f == "part1")), Times.Once);
            _mockDirectoryClient.Verify(
                s => s.GetSubdirectoryClient(It.Is<string>(f => f == "part2")), Times.Once);
            _mockDirectoryClient.Verify(
                s => s.GetSubdirectoryClient(It.Is<string>(f => f == "part3")), Times.Once);
            _mockDirectoryClient.Verify(
                s => s.GetSubdirectoryClient(It.Is<string>(f => f == "file.txt")), Times.Never);
        }     

        [Test]
        public void FileClient_Is_Retrieved()
        {
            _mockDirectoryClient.Verify(x => x.GetFileClient(It.Is<string>(f => f == "file.txt")), Times.Once);
        }

        [Test]
        public void File_Is_Deleted_If_Already_Exists()
        {
            _mockFileClient.Verify(x => x.DeleteIfExistsAsync(null, It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }

        [Test]
        public void File_Is_Created()
        {
            _mockFileClient.Verify(x => x.CreateAsync(It.Is<long>(f => f == 5), null, null, null, null, null, It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }

        [Test]
        public void File_Is_Uploaded()
        {
            _mockFileClient.Verify(x => x.UploadAsync(It.Is<MemoryStream>(f => f != null), null, null, It.Is<CancellationToken>(f => f == _mockCancellationToken)), Times.Once);
        }
    }
}