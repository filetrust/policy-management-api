using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.Extensions.Logging;

namespace Glasswall.PolicyManagement.Business.Store
{
    public class FileStore : IFileStore
    {
        private readonly ILogger<FileStore> _logger;
        private readonly IFileStoreOptions _fileStoreOptions;

        public FileStore(
            ILogger<FileStore> logger,
            IFileStoreOptions fileStoreOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStoreOptions = fileStoreOptions ?? throw new ArgumentNullException(nameof(fileStoreOptions));
        }

        public IAsyncEnumerable<string> SearchAsync(string relativePath, IPathActions pathActions, CancellationToken cancellationToken)
        {
            if (pathActions == null) throw new ArgumentNullException(nameof(pathActions));
            return InternalSearchAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath), pathActions, cancellationToken);
        }

        public Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));
            return InternalExistsAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath));
        }
        
        public Task<MemoryStream> ReadAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));
            
            return InternalReadAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath), cancellationToken);
        }

        public Task WriteAsync(string relativePath, byte[] bytes, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalWriteAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath), bytes);
        }
        
        public Task DeleteAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalDeleteAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath));
        }

        private TResult Retry<TResult>(Func<TResult> action) => _fileStoreOptions.RetryPolicy.ExecuteAsync(() => Task.FromResult(action())).GetAwaiter().GetResult();
        private void Retry(Action action) => _fileStoreOptions.RetryPolicy.ExecuteAsync(() => { action(); return Task.CompletedTask; }).GetAwaiter().GetResult();
        private async Task Retry(Func<Task> action) => await _fileStoreOptions.RetryPolicy.ExecuteAsync(action);

        private Task InternalDeleteAsync(string fullPath)
        {
            if (Retry(() => Directory.Exists(fullPath))) Retry(() => Directory.Delete(fullPath, true));
            if (Retry(() => File.Exists(fullPath))) Retry(() => File.Delete(fullPath));
            return Task.CompletedTask;
        }

        private Task InternalWriteAsync(string fullPath, byte[] bytes)
        {
            var dir = Path.GetDirectoryName(fullPath) ?? throw new ArgumentException("A directory was not specified", nameof(fullPath));

            Retry(() => Directory.CreateDirectory(dir));

            if (Retry(() => File.Exists(fullPath))) 
                Retry(() => File.Delete(fullPath));
            
            Retry(() => File.WriteAllBytes(fullPath, bytes));

            return Task.CompletedTask;
        }

        private async Task<MemoryStream> InternalReadAsync(string path, CancellationToken cancellationToken)
        {
            if (!Retry(() => File.Exists(path)))
                return null;

            var ms = new MemoryStream();

            using (var fs = Retry(() => File.OpenRead(path)))
                await Retry(async () => await fs.CopyToAsync(ms, (int)fs.Length, cancellationToken));

            return ms;
        }

        private Task<bool> InternalExistsAsync(string fullPath)
        {
            return Task.FromResult(
                Retry(() => Directory.Exists(fullPath)) 
             || Retry(() => File.Exists(fullPath)));
        }

        private async IAsyncEnumerable<string> InternalSearchAsync(
            string directory,
            IPathActions pathActions,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            _logger.LogInformation("Searching relativePath '{0}'", directory);

            var subDirectories = Retry(() => Directory.GetDirectories(directory));
            var subFiles = Retry(() => Directory.GetFiles(directory));

            foreach (var subDirectory in subDirectories)
            {
                var relativePath = Collect(subDirectory);
                var action = pathActions.DecideAction(relativePath);

                switch (action)
                {
                    case PathAction.Recurse:
                        await foreach (var subItem in InternalSearchAsync(subDirectory, pathActions, cancellationToken)) yield return subItem;
                        break;
                    case PathAction.Collect:
                        yield return relativePath;
                        break;
                    case PathAction.Break:
                        yield break;
                }
            }

            foreach (var subFile in subFiles)
            {
                var relativePath = Collect(subFile);
                var action = pathActions.DecideAction(relativePath);

                if (action == PathAction.Collect) yield return relativePath;
            }
        }

        private string Collect(string path)
        {
            return path.Replace(_fileStoreOptions.RootPath, "").TrimStart(Retry(() => Path.DirectorySeparatorChar));
        }
    }
}