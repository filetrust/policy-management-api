using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Threading;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.WriteAsync
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
            Assert.That(async() => await ClassInTest.WriteAsync(testPath, null, CancellationToken.None), ThrowsArgumentException("relativePath", "Value must not be null or whitespace"));
        }
    }
}
