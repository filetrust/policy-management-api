using Glasswall.PolicyManagement.Common.Configuration;

namespace Glasswall.PolicyManagement.Business.Configuration
{
    public class PolicyManagementApiConfiguration : IPolicyManagementApiConfiguration
    {
        public string ShareName { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string PolicyUpdateServiceEndpointCsv { get; set; }
        public string TokenUsername { get; set;}
        public string TokenPassword { get; set; }
    }
}