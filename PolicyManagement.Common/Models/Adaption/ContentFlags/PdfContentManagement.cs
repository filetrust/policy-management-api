using Glasswall.PolicyManagement.Common.Models.Enums;

namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    public class PdfContentManagement
    {
        public ContentManagementFlagAction Acroform { get; set; }
        public ContentManagementFlagAction ActionsAll { get; set; }
        public ContentManagementFlagAction EmbeddedFiles { get; set; }
        public ContentManagementFlagAction EmbeddedImages { get; set; }
        public ContentManagementFlagAction ExternalHyperlinks { get; set; }
        public ContentManagementFlagAction InternalHyperlinks { get; set; }
        public ContentManagementFlagAction Javascript { get; set; }
        public ContentManagementFlagAction Metadata { get; set; }
    }
}