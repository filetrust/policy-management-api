using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.ExistsAsync
{
    [TestFixture]
    public class WithExistingDirectory : MountedFileStoreTestBase
    {
        private bool _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            if (!Directory.Exists(Path.Combine(RootPath, "subDirectory")))
            {
                Directory.CreateDirectory(Path.Combine(RootPath, "subDirectory"));
            }
            
            _output = await ClassInTest.ExistsAsync("subDirectory", CancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void True_Is_Returned()
        {
            Assert.That(_output);
        }
    }
}
