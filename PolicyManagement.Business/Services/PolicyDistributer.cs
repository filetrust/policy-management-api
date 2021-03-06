﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Services;
using Microsoft.Extensions.Logging;

namespace Glasswall.PolicyManagement.Business.Services
{
    public class PolicyDistributer : IPolicyDistributer
    {
        private readonly ILogger<PolicyDistributer> _logger;
        private readonly IPolicyManagementApiConfiguration _policyManagementApiConfiguration;
        
        public PolicyDistributer(
            ILogger<PolicyDistributer> logger,
            IPolicyManagementApiConfiguration policyManagementApiConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _policyManagementApiConfiguration = policyManagementApiConfiguration ?? throw new ArgumentNullException(nameof(policyManagementApiConfiguration));
        }

        public Task DistributeAdaptionPolicy(PolicyModel policy, CancellationToken cancellationToken)
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            return InternalDistributeAdaptionPolicy(policy, cancellationToken);
        }

        public Task DistributeNcfsPolicy(PolicyModel policy, CancellationToken cancellationToken)
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            return InternalDistributeNcfsPolicy(policy, cancellationToken);
        }

        private async Task InternalDistributeNcfsPolicy(PolicyModel policy, CancellationToken cancellationToken)
        {
            var payload = new
            {
                GlasswallBlockedFilesAction = policy?.NcfsPolicy?.NcfsActions?.GlasswallBlockedFilesAction ?? NcfsOption.Relay,
                UnprocessableFileTypeAction = policy?.NcfsPolicy?.NcfsActions?.UnprocessableFileTypeAction ?? NcfsOption.Relay
            };

            foreach (var endpoint in _policyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv.Split(','))
            {
                try
                {
                    _logger.LogInformation($"Signalling Policy Update to '{endpoint}' starting. PolicyId: '{policy?.Id}'");

                    FlurlHttp.ConfigureClient(endpoint, cli =>
                        cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

                    var tokenResponse = await $"{endpoint}/api/v1/auth/token".WithBasicAuth(
                        _policyManagementApiConfiguration.NcfsPolicyUpdateServiceUsername,
                        _policyManagementApiConfiguration.NcfsPolicyUpdateServicePassword
                    ).GetAsync(cancellationToken);

                    var token = await tokenResponse.GetStringAsync();

                    await $"{endpoint}/api/v1/policy".WithOAuthBearerToken(token).PutJsonAsync(payload, cancellationToken);

                    _logger.LogInformation($"Signalling Policy Update to '{endpoint}' complete");
                }
                catch (FlurlHttpException ex)
                {
                    _logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {ex.Message}");
                }
            }
        }

        private async Task InternalDistributeAdaptionPolicy(PolicyModel policy, CancellationToken cancellationToken)
        {
            foreach (var endpoint in _policyManagementApiConfiguration.PolicyUpdateServiceEndpointCsv.Split(','))
            {
                try
                {
                    _logger.LogInformation($"Signalling Policy Update to '{endpoint}' starting. PolicyId: '{policy?.Id}'");
                    
                    FlurlHttp.ConfigureClient(endpoint, cli =>
                        cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

                    var tokenResponse = await $"{endpoint}/api/v1/auth/token".WithBasicAuth(
                        _policyManagementApiConfiguration.PolicyUpdateServiceUsername,
                        _policyManagementApiConfiguration.PolicyUpdateServicePassword
                    ).GetAsync(cancellationToken);
                    
                    var token = await tokenResponse.GetStringAsync();

                    await $"{endpoint}/api/v1/policy".WithOAuthBearerToken(token).PutJsonAsync(new
                    {
                        PolicyId = policy.Id,
                        policy.AdaptionPolicy?.ContentManagementFlags,
                        policy.AdaptionPolicy?.NcfsActions?.UnprocessableFileTypeAction,
                        policy.AdaptionPolicy?.NcfsActions?.GlasswallBlockedFilesAction,
                        NcfsRoutingUrl = NcfsRoutingUrlOrDefault(policy.AdaptionPolicy?.NcfsRoute?.NcfsRoutingUrl),
                        RebuildReportMessage = policy.AdaptionPolicy?.ErrorReportTemplate ?? "File could not be rebuilt",
                        ArchivePasswordProtectedReportMessage = policy.AdaptionPolicy?.ArchivePasswordProtectedReportMessage ?? "Archive is password protected and could not be rebuilt",
                        ArchiveErrorReportMessage = policy.AdaptionPolicy?.ArchiveErrorReportMessage ?? "Archive contains an error and could not be rebuilt"
                    }, cancellationToken);

                    _logger.LogInformation($"Signalling Policy Update to '{endpoint}' complete");
                }
                catch (FlurlHttpException ex)
                {
                    _logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {ex.Message}");
                }
            }
        }

        private static string NcfsRoutingUrlOrDefault(string fromPolicy)
        {
            return string.IsNullOrWhiteSpace(fromPolicy) ? "https://ncfs-reference-service.icap-ncfs.svc.cluster.local" : fromPolicy;
        }
    }
}