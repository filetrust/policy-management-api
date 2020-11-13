using Glasswall.PolicyManagement.Common.Models.Enums;

namespace Glasswall.PolicyManagement.Common.Models.Ncfs
{
    public class NcfsActions
    {
        public NcfsOption UnprocessableFileTypeAction { get; set; }
        public NcfsOption GlasswallBlockedFilesAction { get; set; }
    }
}