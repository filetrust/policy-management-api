using System;
using Glasswall.PolicyManagement.Common.Store;
using Polly;

namespace Glasswall.PolicyManagement.Business.Store
{
    public class PolicyStoreOptions : IFileStoreOptions
    {
        public PolicyStoreOptions(string rootPath, AsyncPolicy retryPolicy)
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            RetryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
        }

        public string RootPath { get; }
        public AsyncPolicy RetryPolicy { get; }
    }
}