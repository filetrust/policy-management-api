using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.PolicyManagement.Common.BackgroundServices
{
    public interface IPolicySynchronizer
    {
        Task SyncPoliciesAsync(CancellationToken cancellationToken);
    }
}