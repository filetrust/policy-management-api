using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.DeleteAsync
{
    [TestFixture]
    public class GivenPathAlreadyExists : MountedFileStoreTestBase
    {
        private string _fullPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            var relativePath = $"{Guid.NewGuid()}/{Guid.NewGuid()}.txt";
            _fullPath = $"{RootPath}{Path.DirectorySeparatorChar}{relativePath}";

            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
            File.WriteAllBytes(_fullPath, new byte[] { 0x00, 0x11 });

            await ClassInTest.DeleteAsync(relativePath, CancellationToken);
        }

        [Test]
        public void File_Is_Deleted()
        {
            Assert.That(!File.Exists(_fullPath));
        }
    }
}
