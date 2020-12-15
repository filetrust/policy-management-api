using NUnit.Framework;
using System.Threading;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.DeleteAsync
{
    [TestFixture]
    public class WhenPathIsNullOrWhitespace : MountedFileStoreTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Exception_Is_Thrown(string testPath)
        {
            Assert.That(async() => await ClassInTest.DeleteAsync(testPath, CancellationToken.None), ThrowsArgumentException("relativePath", "Value must not be null or whitespace"));
        }
    }
}
