using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Glasswall.PolicyManagement.Common.Store;

namespace Glasswlal.PolicyManagement.Business.Store
{
    public class AzureFileShare : IFileShare
    {
        private readonly ShareClient _shareClient;

        public AzureFileShare(ShareClient shareClient)
        {
            _shareClient = shareClient ?? throw new ArgumentNullException(nameof(shareClient));
        }

        public IAsyncEnumerable<string> ListAsync(IPathFilter pathFilter, CancellationToken cancellationToken)
        {
            if (pathFilter == null) throw new ArgumentNullException(nameof(pathFilter));
            return RecurseDirectory(_shareClient.GetRootDirectoryClient(), pathFilter, cancellationToken);
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value must not be null or whitespace", nameof(path));
            return InternalExistsAsync(path, cancellationToken);
        }

        public Task<MemoryStream> DownloadAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value must not be null or whitespace", nameof(path));
            return InternalDownloadAsync(path, cancellationToken);
        }

        public Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value must not be null or whitespace", nameof(path));
            return InternalDeleteDirectoryAsync(path, cancellationToken);
        }

        public Task UploadAsync(string path, byte[] bytes, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value must not be null or whitespace", nameof(path));
            return InternalUploadAsync(path, bytes, token);
        }

        private async Task InternalUploadAsync(string path, byte[] bytes, CancellationToken cancellationToken)
        {
            var pathParts = path.Split('/');

            if (pathParts.Length > 1)
            {
                var curDir = _shareClient.GetRootDirectoryClient();
                for (var i = 0; i < pathParts.Length - 1; i++)
                {
                    curDir = curDir.GetSubdirectoryClient(pathParts[i]);
                    await curDir.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                }

                var fileClient = curDir.GetFileClient(pathParts.Last());
                using var ms = new MemoryStream(bytes);
                await fileClient.UploadAsync(ms, cancellationToken: cancellationToken);
            }
        }

        private async Task<MemoryStream> InternalDownloadAsync(string path, CancellationToken cancellationToken)
        {
            var fileClient = _shareClient.GetRootDirectoryClient().GetFileClient(path);

            if (!await InternalExistsAsync(fileClient, cancellationToken))
                return null;

            var ms = new MemoryStream();
            var file = await fileClient.DownloadAsync(cancellationToken: cancellationToken);
            await file.Value.Content.CopyToAsync(ms, (int)file.Value.ContentLength, cancellationToken);
            return ms;
        }

        private async Task<bool> InternalExistsAsync(string path, CancellationToken cancellationToken)
        {
            if (Path.HasExtension(path))
                return await InternalExistsAsync(_shareClient.GetRootDirectoryClient().GetFileClient(path), cancellationToken);

            return await InternalExistsAsync(_shareClient.GetDirectoryClient(path), cancellationToken);
        }

        private static async Task<bool> InternalExistsAsync(ShareDirectoryClient client, CancellationToken token)
        {
            try
            {
                return await client.ExistsAsync(token);
            }
            catch (RequestFailedException rex)
            {
                if (rex.ErrorCode == ShareErrorCode.ParentNotFound)
                    return false;

                throw;
            }
        }

        private static async Task<bool> InternalExistsAsync(ShareFileClient client, CancellationToken token)
        {
            try
            {
                return await client.ExistsAsync(token);
            }
            catch (RequestFailedException rex)
            {
                if (rex.ErrorCode == ShareErrorCode.ParentNotFound)
                    return false;

                throw;
            }
        }

        private async Task InternalDeleteDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            var directoryClient = _shareClient.GetDirectoryClient(path);

            if (!await directoryClient.ExistsAsync(cancellationToken))
                return;

            await DeleteTree(directoryClient, cancellationToken);
        }

        private static async Task DeleteTree(
            ShareDirectoryClient directory,
            CancellationToken cancellationToken)
        {
            await foreach (var item in directory.GetFilesAndDirectoriesAsync(cancellationToken: cancellationToken))
            {
                if (item.IsDirectory)
                {
                    await DeleteTree(directory.GetSubdirectoryClient(item.Name), cancellationToken);
                    await directory.DeleteSubdirectoryAsync(item.Name, cancellationToken);
                }
                else
                {
                    await directory.DeleteFileAsync(item.Name, cancellationToken);
                }
            }

            await directory.DeleteAsync(cancellationToken);
        }

        private static async IAsyncEnumerable<string> RecurseDirectory(
            ShareDirectoryClient directory,
            IPathFilter pathFilter, 
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in directory.GetFilesAndDirectoriesAsync(cancellationToken: cancellationToken))
            {
                var itemPath = GetNextPath(directory.Path, item);

                switch (pathFilter.DecideAction(itemPath))
                {
                    case PathAction.Collect:
                        yield return itemPath;
                        break;
                    case PathAction.Break:
                        yield break;
                    case PathAction.Recurse when item.IsDirectory:
                        var subDirectory = directory.GetSubdirectoryClient(item.Name);
                        await foreach (var subItem in RecurseDirectory(subDirectory, pathFilter, cancellationToken))
                            yield return subItem;
                        break;
                }
            }
        }

        private static string GetNextPath(string directory, ShareFileItem file)
        {
            return $"{directory}{(directory == "" ? "" : "/")}{file.Name}";
        }
    }
}