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
    public class MountedFileStore : IFileStore
    {
        private readonly ILogger<MountedFileStore> _logger;
        private readonly string _mountedFileDirectory;

        public MountedFileStore(
            ILogger<MountedFileStore> logger,
            string mountedFileDirectory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mountedFileDirectory = mountedFileDirectory ?? throw new ArgumentNullException(nameof(mountedFileDirectory));
        }

        public IAsyncEnumerable<string> SearchAsync(string relativePath, IPathActions pathActions, CancellationToken cancellationToken)
        {
            if (pathActions == null) throw new ArgumentNullException(nameof(pathActions));
            return InternalSearchAsync(Path.Combine(_mountedFileDirectory, relativePath), pathActions, cancellationToken);
        }

        public Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));
            return InternalExistsAsync(Path.Combine(_mountedFileDirectory, relativePath));
        }
        
        public Task<MemoryStream> ReadAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));
            
            return InternalReadAsync(Path.Combine(_mountedFileDirectory, relativePath), cancellationToken);
        }

        public Task WriteAsync(string relativePath, byte[] bytes, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalWriteAsync(Path.Combine(_mountedFileDirectory, relativePath), bytes);
        }
        
        public Task DeleteAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalDeleteAsync(Path.Combine(_mountedFileDirectory, relativePath));
        }

        private static Task InternalDeleteAsync(string fullPath)
        {
            if (Directory.Exists(fullPath))
                Directory.Delete(fullPath, true);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        private static Task InternalWriteAsync(string fullPath, byte[] bytes)
        {
            var dir = Path.GetDirectoryName(fullPath)
                      ?? throw new ArgumentException("A directory was not specified", nameof(fullPath));

            Directory.CreateDirectory(dir);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            File.WriteAllBytes(fullPath, bytes);

            return Task.CompletedTask;
        }

        private static async Task<MemoryStream> InternalReadAsync(string path, CancellationToken cancellationToken)
        {
            if (!File.Exists(path))
                return null;

            var ms = new MemoryStream();

            using (var fs = File.OpenRead(path))
            {
                await fs.CopyToAsync(ms, (int)fs.Length, cancellationToken);
            }

            return ms;
        }

        private static Task<bool> InternalExistsAsync(string fullPath)
        {
            return Task.FromResult(Directory.Exists(fullPath) || File.Exists(fullPath));
        }

        private async IAsyncEnumerable<string> InternalSearchAsync(
            string directory,
            IPathActions pathActions,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            _logger.LogInformation("Searching relativePath '{0}'", directory);

            var subDirectories = Directory.GetDirectories(directory);
            var subFiles = Directory.GetFiles(directory);

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
            return path.Replace(_mountedFileDirectory, "").TrimStart(Path.DirectorySeparatorChar);
        }
    }
}