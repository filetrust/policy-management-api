namespace Glasswall.PolicyManagement.Common.Configuration
{
    public interface IPolicyManagementApiConfiguration
    {
        string PolicyUpdateServiceUsername { get; }
        string PolicyUpdateServicePassword { get; }
        string PolicyUpdateServiceEndpointCsv { get; }
        string NcfsPolicyUpdateServiceUsername { get; }
        string NcfsPolicyUpdateServicePassword { get; }
        string NcfsPolicyUpdateServiceEndpointCsv { get; }
    }
}