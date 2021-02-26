using Polly;

namespace Glasswall.PolicyManagement.Common.Store
{
    public interface IFileStoreOptions
    {
        string RootPath { get; }
        AsyncPolicy RetryPolicy { get; }
    }
}