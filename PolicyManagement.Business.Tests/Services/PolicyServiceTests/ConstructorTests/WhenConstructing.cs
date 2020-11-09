using Glasswall.PolicyManagement.Business.Services;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<PolicyService>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<PolicyService>();
        }
    }
}
