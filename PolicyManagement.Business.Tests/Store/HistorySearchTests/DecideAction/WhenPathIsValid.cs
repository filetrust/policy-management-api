using Glasswall.PolicyManagement.Business.Store;
using Glasswall.PolicyManagement.Common.Store;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Store.HistorySearchTests.DecideAction
{
    [TestFixture]
    public class WhenPathIsValid : UnitTestBase<IPathActions>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ClassInTest = new HistorySearch();
        }

        [Test]
        [TestCase("historical", PathAction.Recurse)]
        [TestCase("historical/4ced533c-7977-4333-9f22-f92ea3985fe3", PathAction.Collect)]
        [TestCase("4p5k2t08g", PathAction.Continue)]
        public void Action_Is_Correct(string path, PathAction expectedAction)
        {
            var output = ClassInTest.DecideAction(path);

            Assert.That(output, Is.EqualTo(expectedAction), $"{path} - {expectedAction}");
        }
    }
}
