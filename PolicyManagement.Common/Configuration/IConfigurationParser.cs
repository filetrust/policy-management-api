namespace Glasswall.PolicyManagement.Common.Configuration
{
    public interface IConfigurationParser
    {
        TConfiguration Parse<TConfiguration>() where TConfiguration : new();
    }
}