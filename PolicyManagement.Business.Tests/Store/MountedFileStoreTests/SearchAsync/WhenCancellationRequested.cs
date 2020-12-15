using Glasswall.PolicyManagement.Common.Store;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.SearchAsync
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class WhenCancellationRequested : MountedFileStoreTestBase
    {
        private List<string> _paths;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _paths = new List<string>();

            await foreach (var path in ClassInTest.SearchAsync("", new RecurseAllActionDecider(), new CancellationToken(true)))
            {
                _paths.Add(path);
            }
        }
        
        [Test]
        public void Paths_Are_Returned()
        {
            Assert.That(_paths, Is.Empty);
        }

        private class RecurseAllActionDecider : IPathActions
        {
            public PathAction DecideAction(string path)
            {
                return PathAction.Collect;
            }
        }
    }
}
