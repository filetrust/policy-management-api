using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;

namespace Glasswall.PolicyManagement.Common.Services
{
    public interface IPolicyDistributer
    {
        Task DistributeAdaptionPolicy(PolicyModel policy, CancellationToken cancellationToken);
        Task DistributeNcfsPolicy(PolicyModel policy, CancellationToken cancellationToken);
    }
}