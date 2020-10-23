using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;

namespace Glasswall.PolicyManagement.Common.Services
{
    public interface IPolicyService
    {
        Task<PolicyModel> GetDraftAsync();
        
        Task<PolicyModel> GetCurrentAsync();
        
        Task<IEnumerable<PolicyModel>> GetHistoricalPoliciesAsync();
        
        Task<PolicyModel> GetPolicyByIdAsync(Guid id);

        Task PublishAsync();

        Task SaveAsync(PolicyModel policyModel);
        
        Task DeleteAsync(Guid id);
    }
}