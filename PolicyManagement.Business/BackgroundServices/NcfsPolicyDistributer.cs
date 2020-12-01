using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.BackgroundServices;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Services;
using Microsoft.Extensions.Logging;

namespace Glasswall.PolicyManagement.Business.BackgroundServices
{
    public class NcfsPolicyDistributer : IPolicySynchronizer
    {
        private static string _endpointCache;

        private readonly ILogger<NcfsPolicyDistributer> _logger;
        private readonly IPolicyManagementApiConfiguration _policyManagementApiConfiguration;
        private readonly IPolicyService _policyService;
        private readonly IPolicyDistributer _policyDistributer;

        public NcfsPolicyDistributer(
            ILogger<NcfsPolicyDistributer> logger,
            IPolicyManagementApiConfiguration policyManagementApiConfiguration,
            IPolicyService policyService,
            IPolicyDistributer policyDistributer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _policyManagementApiConfiguration = policyManagementApiConfiguration ?? throw new ArgumentNullException(nameof(policyManagementApiConfiguration));
            _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
            _policyDistributer = policyDistributer ?? throw new ArgumentNullException(nameof(policyDistributer));
        }

        public async Task SyncPoliciesAsync(CancellationToken ct)
        {
            if (_policyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv != _endpointCache)
            {
                _logger.LogInformation("{0}: endpoints have changed. '{1}' -> '{2}'. Redistributing policy", nameof(NcfsPolicyDistributer), _endpointCache, _policyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv);

                var currentPolicy = await _policyService.GetCurrentAsync(ct);

                if (currentPolicy == null)
                {
                    _logger.LogWarning("{0}: no current policy found.", nameof(NcfsPolicyDistributer));
                    return;
                }

                await _policyDistributer.DistributeNcfsPolicy(currentPolicy, ct);
                _logger.LogInformation("{0}: policy '{1}' has been distributed.", nameof(NcfsPolicyDistributer), currentPolicy.Id);
                _endpointCache = _policyManagementApiConfiguration.NcfsPolicyUpdateServiceEndpointCsv;
            }
        }
    }
}
