using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.PolicyManagement.Common.Store
{
    /// <summary>
    /// Provides abstraction of the underlying file share
    /// </summary>
    public interface IFileStore
    {
        /// <summary>
        /// Retrieves a list of paths matching the filter
        /// </summary>
        /// <param name="relativePath">Object path relative to the root of the store</param>
        /// <param name="pathActions">An object that takes the responsibility of recursing the directory</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of path matches</returns>
        IAsyncEnumerable<string> SearchAsync(string relativePath, IPathActions pathActions, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether the path exists
        /// </summary>
        /// <param name="relativePath">Object path relative to the root of the store</param>
        /// <param name="cancellationToken">Token to cancel the work</param>
        /// <returns>True if so, false otherwise</returns>
        Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken);

        /// <summary>
        /// Downloads the object at the path specified
        /// </summary>
        /// <param name="relativePath">Object path relative to the root of the store</param>
        /// <param name="cancellationToken">Token to cancel the work</param>
        /// <returns>A memory stream containing the data</returns>
        Task<MemoryStream> ReadAsync(string relativePath, CancellationToken cancellationToken);

        /// <summary>
        /// Saves a file asynchronously
        /// </summary>
        /// <param name="relativePath">Object path relative to the root of the store</param>
        /// <param name="bytes">Raw data for the file</param>
        /// <param name="cancellationToken">Token to cancel the work</param>
        /// <returns>A task representing the work to be carried out</returns>
        Task WriteAsync(string relativePath, byte[] bytes, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a directory or file asynchronously
        /// </summary>
        /// <param name="relativePath">Object path relative to the root of the store</param>
        /// <param name="cancellationToken">Token to cancel the work</param>
        /// <returns>A task representing the work to be carried out</returns>
        Task DeleteAsync(string relativePath, CancellationToken cancellationToken);
    }
}