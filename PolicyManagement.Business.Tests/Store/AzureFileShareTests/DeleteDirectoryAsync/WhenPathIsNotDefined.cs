using System.Threading;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Store.AzureFileShareTests.DeleteDirectoryAsync
{
    [TestFixture]
    public class WhenPathIsNotDefined : AzureFileShareTestBase
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
        public void Exception_Is_Thrown(string path)
        {
            Assert.That(
                async () => await ClassInTest.DeleteDirectoryAsync(path, CancellationToken.None),
                ThrowsArgumentException("path", "Value must not be null or whitespace"));
        }
    }
}
