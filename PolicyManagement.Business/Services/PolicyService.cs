using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Serialisation;
using Glasswall.PolicyManagement.Common.Services;
using Glasswall.PolicyManagement.Common.Store;
using Glasswlal.PolicyManagement.Business.Store;

namespace Glasswlal.PolicyManagement.Business.Services
{
    public class PolicyService : IPolicyService
    {
        private const string PolicyFileName = "policy.json";
        private const string CurrentPolicyPath = "current/" + PolicyFileName;
        private const string DraftPolicyPath = "draft/" + PolicyFileName;
        private const string HistoryPathTemplate = "historical/{0}/" + PolicyFileName;

        private readonly IFileShare _fileShare;
        private readonly IJsonSerialiser _jsonSerialiser;

        public PolicyService(
            IFileShare fileShare,
            IJsonSerialiser jsonSerialiser)
        {
            _fileShare = fileShare ?? throw new ArgumentNullException(nameof(fileShare));
            _jsonSerialiser = jsonSerialiser ?? throw new ArgumentNullException(nameof(jsonSerialiser));
        }

        public async Task<PolicyModel> GetDraftAsync(CancellationToken token)
        {
            return await InternalDownloadAsync(DraftPolicyPath, token);
        }

        public async Task<PolicyModel> GetCurrentAsync(CancellationToken token)
        {
            return await InternalDownloadAsync(CurrentPolicyPath, token);
        }

        public async IAsyncEnumerable<PolicyModel> GetHistoricalPoliciesAsync([EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var path in _fileShare.ListAsync(new HistorySearch(), token))
                yield return await InternalDownloadAsync(path, token);
        }

        public async Task<PolicyModel> GetPolicyByIdAsync(Guid id, CancellationToken token)
        {
            await foreach (var path in _fileShare.ListAsync(new HistorySearchById(id), token))
                return await InternalDownloadAsync($"{path}/{PolicyFileName}", token);

            var currentPolicy = await GetCurrentAsync(token);
            if (currentPolicy?.Id == id) return currentPolicy;

            var draftPolicy = await GetDraftAsync(token);
            return draftPolicy?.Id == id ? draftPolicy : null;
        }

        public async Task PublishAsync(CancellationToken token)
        {
            var draft = await GetDraftAsync(token);
            if (draft == null) return;

            var current = await GetCurrentAsync(token);
            if (current == null) return;

            current.PolicyState = PolicyState.Historical;
            draft.PolicyState = PolicyState.Published;

            await InternalUploadPolicyAsync(string.Format(HistoryPathTemplate, current.Id), current, token);
            await InternalUploadPolicyAsync(CurrentPolicyPath, draft, token);
            await _fileShare.DeleteDirectoryAsync(DraftPolicyPath, token);
        }

        public Task SaveAsync(PolicyModel policyModel, CancellationToken token)
        {
            if (policyModel == null) throw new ArgumentNullException(nameof(policyModel));
            return InternalSaveAsync(policyModel, token);
        }
        
        public async Task DeleteAsync(Guid id, CancellationToken token)
        {
            await foreach (var path in _fileShare.ListAsync(new HistorySearchById(id), token))
            {
                await _fileShare.DeleteDirectoryAsync(path, token);
                return;
            }
        }

        private async Task<PolicyModel> InternalDownloadAsync(string path, CancellationToken ct)
        {
            if (!await _fileShare.ExistsAsync(path, ct))
                return null;

            var ms = await _fileShare.DownloadAsync(path, ct);
            return await _jsonSerialiser.Deserialize<PolicyModel>(ms, ct);
        }

        private async Task InternalSaveAsync(PolicyModel policyModel, CancellationToken cancellationToken)
        {
            policyModel.Timestamp = DateTimeOffset.UtcNow;
            await InternalUploadPolicyAsync(DraftPolicyPath, policyModel, cancellationToken);
        }

        private async Task InternalUploadPolicyAsync(string path, PolicyModel policyModel, CancellationToken cancellationToken)
        {
            var jsonFile = await _jsonSerialiser.Serialise(policyModel, cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(jsonFile);
            await _fileShare.UploadAsync(path, bytes, cancellationToken);
        }
    }
}