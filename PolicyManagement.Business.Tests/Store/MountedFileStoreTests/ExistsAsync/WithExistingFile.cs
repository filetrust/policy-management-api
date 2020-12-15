using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.ExistsAsync
{
    [TestFixture]
    public class WithExistingFile : MountedFileStoreTestBase
    {
        private bool _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }

            var relativePath = $"{Guid.NewGuid()}.txt";

            if (!File.Exists(relativePath))
            {
                await File.WriteAllTextAsync($"{RootPath}{Path.DirectorySeparatorChar}{relativePath}", "some text", CancellationToken);
            }

            _output = await ClassInTest.ExistsAsync(relativePath, CancellationToken);
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
