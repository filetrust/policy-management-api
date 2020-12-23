using System.Diagnostics.CodeAnalysis;
using Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags;
using Glasswall.PolicyManagement.Common.Models.Ncfs;

namespace Glasswall.PolicyManagement.Common.Models.Adaption
{
    [ExcludeFromCodeCoverage]
    public class AdaptionPolicy
    {
        public ContentManagementFlags ContentManagementFlags { get; set; }
        public string ErrorReportTemplate { get; set; }
        public string ArchivePasswordProtectedReportMessage { get; set; }
        public string ArchiveErrorReportMessage { get; set; }
        public NcfsRoute NcfsRoute { get; set; }
        public NcfsActions NcfsActions { get; set; }
    }
}