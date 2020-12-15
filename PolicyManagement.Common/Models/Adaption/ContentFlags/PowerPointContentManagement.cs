using Glasswall.PolicyManagement.Common.Models.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    [ExcludeFromCodeCoverage]
    public class PowerPointContentManagement
    {
        public ContentManagementFlagAction EmbeddedFiles { get; set; }
        public ContentManagementFlagAction EmbeddedImages { get; set; }
        public ContentManagementFlagAction ExternalHyperlinks { get; set; }
        public ContentManagementFlagAction InternalHyperlinks { get; set; }
        public ContentManagementFlagAction Macros { get; set; }
        public ContentManagementFlagAction Metadata { get; set; }
        public ContentManagementFlagAction ReviewComments { get; set; }
    }
}