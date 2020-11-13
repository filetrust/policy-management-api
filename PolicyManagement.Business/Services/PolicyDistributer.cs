using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Models;
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
            foreach (var endpoint in _policyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv.Split(','))
            {
                try
                {
                    _logger.LogInformation($"Signalling Policy Update to '{endpoint}' starting");

                    FlurlHttp.ConfigureClient(endpoint, cli =>
                        cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

                    var tokenResponse = await $"{endpoint}/api/v1/auth/token".WithBasicAuth(
                        _policyManagementApiConfiguration.TokenUsername,
                        _policyManagementApiConfiguration.TokenPassword
                    ).GetAsync(cancellationToken);

                    var token = await tokenResponse.GetStringAsync();

                    await $"{endpoint}/api/v1/policy".WithOAuthBearerToken(token).PutJsonAsync(new
                    {
                        policy.NcfsPolicy?.NcfsDecision
                    }, cancellationToken);

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
                    _logger.LogInformation($"Signalling Policy Update to '{endpoint}' starting");
                    
                    FlurlHttp.ConfigureClient(endpoint, cli =>
                        cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

                    var tokenResponse = await $"{endpoint}/api/v1/auth/token".WithBasicAuth(
                        _policyManagementApiConfiguration.TokenUsername,
                        _policyManagementApiConfiguration.TokenPassword
                    ).GetAsync(cancellationToken);
                    
                    var token = await tokenResponse.GetStringAsync();

                    await $"{endpoint}/api/v1/policy".WithOAuthBearerToken(token).PutJsonAsync(new
                    {
                        PolicyId = policy.Id,
                        policy.AdaptionPolicy?.ContentManagementFlags,
                        policy.AdaptionPolicy?.NcfsActions?.UnprocessableFileTypeAction,
                        policy.AdaptionPolicy?.NcfsActions?.GlasswallBlockedFilesAction,
                        policy.AdaptionPolicy?.NcfsRoute?.NcfsRoutingUrl,
                        policy.AdaptionPolicy?.ErrorReportTemplate
                    }, cancellationToken);

                    _logger.LogInformation($"Signalling Policy Update to '{endpoint}' complete");
                }
                catch (FlurlHttpException ex)
                {
                    _logger.LogCritical($"Error returned from {ex.Call.Request.Url}: {ex.Message}");
                }
            }
        }
    }
}