using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Glasswall.PolicyManagement.Api.BackgroundServices
{
    public class PolicySynchronizationBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger<PolicySynchronizationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public PolicySynchronizationBackgroundService(
            ILogger<PolicySynchronizationBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{0}: Timer Started.", nameof(PolicySynchronizationBackgroundService));
            _timer = new Timer(_ => { DistributeCurrentPolicy(stoppingToken).GetAwaiter().GetResult(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{0}: Timer Stopping.", nameof(PolicySynchronizationBackgroundService));

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task DistributeCurrentPolicy(CancellationToken ct)
        {
            _logger.LogInformation("{0}: Synchronizing cluster policies.", nameof(PolicySynchronizationBackgroundService));

            using (var scope = _serviceProvider.CreateScope())
            {
                var synchronizers = scope.ServiceProvider.GetRequiredService<IEnumerable<IPolicySynchronizer>>();

                foreach (var synchronizer in synchronizers)
                {
                    try
                    {
                        await synchronizer.SyncPoliciesAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(0, ex, "{0}: Cluster policy could not be synchronized.", nameof(PolicySynchronizationBackgroundService));
                    }
                }
            }

            _logger.LogInformation("{0}: Synchronized cluster policies.", nameof(PolicySynchronizationBackgroundService));
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
