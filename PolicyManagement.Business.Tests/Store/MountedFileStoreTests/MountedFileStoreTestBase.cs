using System;
using System.IO;
using System.Threading;
using Glasswall.PolicyManagement.Business.Store;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using TestCommon;

namespace PolicyManagement.Business.Tests.Store.MountedFileStoreTests
{
    public class MountedFileStoreTestBase : UnitTestBase<FileStore>
    {
        protected string RootPath;
        protected Mock<ILogger<FileStore>> Logger;
        protected Mock<IFileStoreOptions> FileStoreOptions;
        protected CancellationToken CancellationToken;

        public void SharedSetup(string rootPath = null)
        {
            rootPath ??= $".{Path.DirectorySeparatorChar}{Guid.NewGuid()}";
            RootPath = rootPath;
            Logger = new Mock<ILogger<FileStore>>();
            FileStoreOptions = new Mock<IFileStoreOptions>();
            CancellationToken = new CancellationToken(false);

            FileStoreOptions.Setup(s => s.RetryPolicy)
                .Returns(Policy.Handle<Exception>().RetryAsync());

            FileStoreOptions.Setup(s => s.RootPath)
                .Returns(RootPath);

            ClassInTest = new FileStore(Logger.Object, FileStoreOptions.Object);
        }
    }
}
