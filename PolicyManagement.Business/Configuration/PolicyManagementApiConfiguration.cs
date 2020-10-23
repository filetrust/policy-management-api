using Glasswall.PolicyManagement.Common.Configuration;

namespace Glasswlal.PolicyManagement.Business.Configuration
{
    public class PolicyManagementApiConfiguration : IPolicyManagementApiConfiguration
    {
        public string ShareName { get; set; }
        public string AzureStorageConnectionString { get; set; }
    }
}