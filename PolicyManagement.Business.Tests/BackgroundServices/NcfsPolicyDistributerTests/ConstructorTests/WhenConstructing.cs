using Glasswall.PolicyManagement.Business.BackgroundServices;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.BackgroundServices.NcfsPolicyDistributerTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<NcfsPolicyDistributer>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<NcfsPolicyDistributer>();
        }
    }
}
