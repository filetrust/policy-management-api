using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Services;

namespace Glasswlal.PolicyManagement.Business.Services
{
    public class PolicyService : IPolicyService
    {
        public Task<PolicyModel> GetDraftAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PolicyModel> GetCurrentAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PolicyModel>> GetHistoricalPoliciesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PolicyModel> GetPolicyByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(PolicyModel policyModel)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}