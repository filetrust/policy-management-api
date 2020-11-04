using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;

namespace Glasswall.PolicyManagement.Common.Services
{
    public interface IPolicyService
    {
        Task<PolicyModel> GetDraftAsync(CancellationToken token);
        
        Task<PolicyModel> GetCurrentAsync(CancellationToken token);
        
        IAsyncEnumerable<PolicyModel> GetHistoricalPoliciesAsync(CancellationToken token);
        
        Task<PolicyModel> GetPolicyByIdAsync(Guid id, CancellationToken token);

        Task PublishAsync(CancellationToken token);

        Task SaveAsync(PolicyModel policyModel, CancellationToken token);
        
        Task DeleteAsync(Guid id, CancellationToken token);
    }
}