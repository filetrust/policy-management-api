using Glasswall.PolicyManagement.Common.Store;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.SearchAsync
{
    [TestFixture]
    public class WhenDirectoriesAreCollected : MountedFileStoreTestBase
    {
        private List<string> _paths;
        private string _fullPath1;
        private string _fullPath2;
        private string _subPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _paths = new List<string>();

            _subPath = $"{Guid.NewGuid()}";
            Directory.CreateDirectory($"{RootPath}{Path.DirectorySeparatorChar}{_subPath}");
            _fullPath1 = $"{RootPath}{Path.DirectorySeparatorChar}{_subPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.txt";
            _fullPath2 = $"{RootPath}{Path.DirectorySeparatorChar}{_subPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.txt";
            await File.WriteAllTextAsync(_fullPath1, "some text", CancellationToken);
            await File.WriteAllTextAsync(_fullPath2, "some text", CancellationToken);

            await foreach (var val in ClassInTest.SearchAsync("", new RecurseAllActionDecider(), CancellationToken))
                _paths.Add(val);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void Paths_Are_Returned()
        {
            Assert.That(_paths.Any(f => f.Contains(_subPath)));
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
