using System.Collections.Generic;

namespace Glasswall.PolicyManagement.Common.Models
{
    public class HistoryResponse
    {
        public int PoliciesCount { get; set; }
        public List<PolicyModel> Policies { get; set; }
    }
}