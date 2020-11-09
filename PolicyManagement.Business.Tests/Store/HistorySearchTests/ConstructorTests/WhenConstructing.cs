using Glasswall.PolicyManagement.Business.Store;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Store.HistorySearchTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing : UnitTestBase<HistorySearch>
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<HistorySearch>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            Assert.That(() => new HistorySearch(), Throws.Nothing);
        }
    }
}
