namespace Glasswall.PolicyManagement.Common.Configuration
{
    public interface IPolicyManagementApiConfiguration
    {
        string ShareName { get; }
        string AzureStorageConnectionString { get; set; }
    }
}