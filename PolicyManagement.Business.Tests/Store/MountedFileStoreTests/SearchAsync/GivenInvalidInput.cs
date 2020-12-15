using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.SearchAsync
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
        public void Exception_Is_Thrown()
        {
            Assert.That(() => ClassInTest.SearchAsync("", null, CancellationToken),
                ThrowsArgumentNullException("pathActions"));
        }
    }
}
