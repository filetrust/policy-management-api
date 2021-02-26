using Glasswall.PolicyManagement.Business.Store;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing : UnitTestBase<FileStore>
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            Assert.That(() => new FileStore(null, Mock.Of<IFileStoreOptions>()), ThrowsArgumentNullException("logger"));
            Assert.That(() => new FileStore(Mock.Of<ILogger<FileStore>>(), null), ThrowsArgumentNullException("fileStoreOptions"));
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            Assert.That(() => new FileStore(Mock.Of<ILogger<FileStore>>(), Mock.Of<IFileStoreOptions>()), Throws.Nothing);
        }
    }
}
