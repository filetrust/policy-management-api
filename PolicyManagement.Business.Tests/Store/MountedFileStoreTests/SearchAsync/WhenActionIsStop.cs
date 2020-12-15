using Glasswall.PolicyManagement.Common.Store;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests.SearchAsync
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class WhenActionIsStop : MountedFileStoreTestBase
    {
        private List<string> _paths;
        private string _subPath;
        private string _fullPath1;
        private string _fullPath2;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _paths = new List<string>();

            Directory.CreateDirectory(RootPath);
            _subPath = $"{RootPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}";
            Directory.CreateDirectory(_subPath);
            _fullPath1 = $"{_subPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.txt";
            _fullPath2 = $"{_subPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.txt";
            await File.WriteAllTextAsync(_fullPath1, "some text", CancellationToken);
            await File.WriteAllTextAsync(_fullPath2, "some text", CancellationToken);

            await foreach (var path in ClassInTest.SearchAsync("", new StopAllActionDecider(), new CancellationToken(false)))
            {
                _paths.Add(path);
            }
        }
        
        [Test]
        public void Paths_Are_Returned()
        {
            Assert.That(_paths, Is.Empty);
        }

        private class StopAllActionDecider : IPathActions
        {
            public PathAction DecideAction(string path)
            {
                return PathAction.Break;
            }
        }
    }
}
