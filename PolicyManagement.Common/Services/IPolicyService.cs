using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;

namespace Glasswall.PolicyManagement.Common.Services
{
    public interface IPolicyService
    {
        Task<PolicyModel> GetDraftAsync(CancellationToken cancellationToken);
        
        Task<PolicyModel> GetCurrentAsync(CancellationToken cancellationToken);
        
        IAsyncEnumerable<PolicyModel> GetHistoricalPoliciesAsync(CancellationToken cancellationToken);

        Task<int> CountHistoricalPoliciesAsync(CancellationToken cancellationToken);

        Task<PolicyModel> GetPolicyByIdAsync(Guid id, CancellationToken cancellationToken);

        Task PublishAsync(Guid id, CancellationToken cancellationToken);

        Task SaveAsDraftAsync(PolicyModel policyModel, CancellationToken cancellationToken);
        
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}