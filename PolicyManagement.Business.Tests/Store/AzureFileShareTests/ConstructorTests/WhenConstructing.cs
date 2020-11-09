using Glasswall.PolicyManagement.Business.Store;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Store.AzureFileShareTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<AzureFileShare>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<AzureFileShare>();
        }
    }
}
