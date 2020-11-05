using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Business.Store;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Serialisation;
using Glasswall.PolicyManagement.Common.Services;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.Extensions.Logging;

namespace Glasswall.PolicyManagement.Business.Services
{
    public class PolicyService : IPolicyService
    {
        private const string PolicyFileName = "policy.json";
        private const string CurrentDirPath = "current";
        private const string CurrentPolicyPath = CurrentDirPath + "/" + PolicyFileName;
        private const string DraftDirPath = "draft";
        private const string DraftPolicyPath = DraftDirPath + "/" + PolicyFileName;
        private const string HistoryFileTemplate = HistoryDirTemplate +"/" + PolicyFileName;
        private const string HistoryDirTemplate = "historical/{0}";

        private readonly IFileShare _fileShare;
        private readonly ILogger<PolicyService> _logger;
        private readonly IJsonSerialiser _jsonSerialiser;

        public PolicyService(
            IFileShare fileShare,
            ILogger<PolicyService> logger,
            IJsonSerialiser jsonSerialiser)
        {
            _fileShare = fileShare ?? throw new ArgumentNullException(nameof(fileShare));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonSerialiser = jsonSerialiser ?? throw new ArgumentNullException(nameof(jsonSerialiser));
        }

        public async Task<PolicyModel> GetDraftAsync(CancellationToken cancellationToken)
        {
            var draft = await InternalDownloadAsync(DraftPolicyPath, cancellationToken);

            if (draft != null) return draft;
            
            var current = await GetCurrentAsync(cancellationToken);
            return current.ToDraft();
        }

        public async Task<PolicyModel> GetCurrentAsync(CancellationToken cancellationToken)
        {
            return await InternalDownloadAsync(CurrentPolicyPath, cancellationToken);
        }

        public async IAsyncEnumerable<PolicyModel> GetHistoricalPoliciesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var path in _fileShare.ListAsync(new HistorySearch(), cancellationToken))
                yield return await InternalDownloadAsync($"{path}/{PolicyFileName}", cancellationToken);
        }
        
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (await _fileShare.ExistsAsync(string.Format(HistoryDirTemplate, id), cancellationToken))
            {
                await _fileShare.DeleteDirectoryAsync(string.Format(HistoryDirTemplate, id), cancellationToken);
            }
            else
            {
                var draftPolicy = await GetDraftAsync(cancellationToken);
                if (draftPolicy.Id == id) await _fileShare.DeleteDirectoryAsync(DraftDirPath, cancellationToken);
            }

            _logger.LogInformation($"Deleted policy {id}.");
        }

        public async Task<PolicyModel> GetPolicyByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var historicPolicyById = await InternalDownloadAsync(string.Format(HistoryFileTemplate, id), cancellationToken);
            if (historicPolicyById != null) return historicPolicyById;

            var currentPolicy = await GetCurrentAsync(cancellationToken);
            if (currentPolicy?.Id == id) return currentPolicy;

            var draftPolicy = await GetDraftAsync(cancellationToken);
            return draftPolicy?.Id == id ? draftPolicy : null;
        }

        public async Task PublishAsync(Guid id, CancellationToken cancellationToken)
        {
            var policyToPublish = await GetPolicyByIdAsync(id, cancellationToken);

            if (policyToPublish == null || policyToPublish.PolicyType == PolicyType.Current)
                return;
            
            var currentPolicy = await GetCurrentAsync(cancellationToken);
            var policyToPublishType = policyToPublish.PolicyType;
            policyToPublish.PolicyType = PolicyType.Current;
            policyToPublish.Published = DateTimeOffset.UtcNow;
            
            await InternalUploadPolicyAsync(CurrentPolicyPath, policyToPublish, cancellationToken);

            if (policyToPublishType == PolicyType.Historical)
                await _fileShare.DeleteDirectoryAsync(string.Format(HistoryDirTemplate, id), cancellationToken);
            else
                await _fileShare.DeleteDirectoryAsync(DraftDirPath, cancellationToken);

            currentPolicy.PolicyType = PolicyType.Historical;
            await InternalUploadPolicyAsync(string.Format(HistoryFileTemplate, currentPolicy.Id), currentPolicy, cancellationToken);
        }

        public Task SaveAsDraftAsync(PolicyModel policyModel, CancellationToken cancellationToken)
        {
            if (policyModel == null) throw new ArgumentNullException(nameof(policyModel));
            return InternalSaveAsDraftAsync(policyModel, cancellationToken);
        }

        private async Task<PolicyModel> InternalDownloadAsync(string path, CancellationToken ct)
        {
            if (!await _fileShare.ExistsAsync(path, ct))
                return null;

            var ms = await _fileShare.DownloadAsync(path, ct);
            return await _jsonSerialiser.Deserialize<PolicyModel>(ms, ct);
        }

        private async Task InternalSaveAsDraftAsync(PolicyModel policyModel, CancellationToken cancellationToken)
        {
            policyModel.LastEdited = DateTimeOffset.UtcNow;
            await InternalUploadPolicyAsync(DraftPolicyPath, policyModel, cancellationToken);
        }

        private async Task InternalUploadPolicyAsync(string fullPolicyJsonPath, PolicyModel policyModel, CancellationToken cancellationToken)
        {
            var jsonFile = await _jsonSerialiser.Serialise(policyModel, cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(jsonFile);
            await _fileShare.UploadAsync(fullPolicyJsonPath, bytes, cancellationToken);
        }
    }
}