using Glasswall.PolicyManagement.Common.Configuration;

namespace Glasswall.PolicyManagement.Business.Configuration
{
    public class PolicyManagementApiConfiguration : IPolicyManagementApiConfiguration
    {
        public string PolicyUpdateServiceEndpointCsv { get; set; }
        public string PolicyUpdateServiceUsername { get; set;}
        public string PolicyUpdateServicePassword { get; set; }
        public string NcfsPolicyUpdateServiceEndpointCsv { get; set; }
        public string NcfsPolicyUpdateServiceUsername { get; set; }
        public string NcfsPolicyUpdateServicePassword { get; set; }
    }
}