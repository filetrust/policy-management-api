using System.Diagnostics.CodeAnalysis;

namespace Glasswall.PolicyManagement.Common.Models.Ncfs
{
    [ExcludeFromCodeCoverage]
    public class NcfsRoute
    {
        public string NcfsRoutingUrl { get; set; }

        public bool IsDeleted { get; set; }
        
        public bool IsValidated { get; set; }
    }
}