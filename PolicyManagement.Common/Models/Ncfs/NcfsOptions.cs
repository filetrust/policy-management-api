using Glasswall.PolicyManagement.Common.Models.Enums;

namespace Glasswall.PolicyManagement.Common.Models.Ncfs
{
    public class NcfsOptions
    {
        public NcfsOption UnProcessableFileTypes { get; set; }
        public NcfsOption GlasswallBlockedFiles { get; set; }
    }
}