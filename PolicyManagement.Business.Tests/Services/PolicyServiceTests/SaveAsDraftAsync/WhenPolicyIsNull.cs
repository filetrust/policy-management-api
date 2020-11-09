using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.SaveAsDraftAsync
{
    [TestFixture]
    public class WhenPolicyIsNull : PolicyServiceTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();
        }

        [Test]
        public void Exception_Is_Thrown()
        {
            Assert.That(() => ClassInTest.SaveAsDraftAsync(null, Token), ThrowsArgumentNullException("policyModel"));
        }
    }
}
