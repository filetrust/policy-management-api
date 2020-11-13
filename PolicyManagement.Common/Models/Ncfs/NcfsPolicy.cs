namespace Glasswall.PolicyManagement.Common.Models.Ncfs
{
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