using System.Diagnostics.CodeAnalysis;

namespace Glasswall.PolicyManagement.Common.Models.Ncfs
{
    [ExcludeFromCodeCoverage]
    public class NcfsPolicy
    {
        public NcfsDecision NcfsDecision { get; set; }
    }

    public enum NcfsDecision
    {
        Relay,
        Replace,
        Block
    }
}