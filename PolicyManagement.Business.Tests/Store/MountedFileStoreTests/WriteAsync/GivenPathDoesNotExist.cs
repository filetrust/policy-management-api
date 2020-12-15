using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.WriteAsync
{
    [TestFixture]
    public class GivenPathDoesNotExist : MountedFileStoreTestBase
    {
        private byte[] _expectedBytes;
        private string _fullPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            var relativePath = $"{Guid.NewGuid()}/{Guid.NewGuid()}.txt";

            _fullPath = $"{RootPath}{Path.DirectorySeparatorChar}{relativePath}";

            await ClassInTest.WriteAsync(relativePath, _expectedBytes = new byte[] { 0x00, 0x22}, CancellationToken);
        }

        [Test]
        public void File_Is_Written()
        {
            CollectionAssert.AreEqual(_expectedBytes, File.ReadAllBytes(_fullPath));
        }
    }
}
