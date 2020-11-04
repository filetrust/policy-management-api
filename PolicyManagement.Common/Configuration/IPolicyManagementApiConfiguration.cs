namespace Glasswall.PolicyManagement.Common.Configuration
{
    public interface IPolicyManagementApiConfiguration
    {
        string ShareName { get; }
        string AccountName { get; set; }
        string AccountKey { get; set; }
        string PolicyUpdateServiceEndpointCsv { get; set; }
    }
}