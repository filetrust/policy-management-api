using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;

namespace Glasswall.PolicyManagement.Common.Services
{
    public interface IPolicyDistributer
    {
        Task Distribute(PolicyModel policy, CancellationToken cancellationToken);
    }
}