namespace Glasswall.PolicyManagement.Common.Configuration
{
    public interface IPolicyManagementApiConfiguration
    {
        string ShareName { get; }
        string AccountName { get; }
        string AccountKey { get; }
        string PolicyUpdateServiceEndpointCsv { get; }
        string TokenUsername { get; }
        string TokenPassword { get; }
    }
}