using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.ReadAsync
{
    [TestFixture]
    public class GivenInvalidInput : MountedFileStoreTestBase
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
        public void Exception_Is_Thrown(string input)
        {
            Assert.That(async () => await ClassInTest.ReadAsync(input, CancellationToken),
                ThrowsArgumentException("relativePath", "Value must not be null or whitespace"));
        }
    }
}
