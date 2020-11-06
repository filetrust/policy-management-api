using Glasswall.PolicyManagement.Common.Models.Enums;

namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    public class ExcelContentFlags
    {
        public ContentManagementFlagAction DynamicDataExchange { get; set; }
        public ContentManagementFlagAction EmbeddedFiles { get; set; }
        public ContentManagementFlagAction EmbeddedImages { get; set; }
        public ContentManagementFlagAction ExternalHyperlinks { get; set; }
        public ContentManagementFlagAction InternalHyperlinks { get; set; }
        public ContentManagementFlagAction Macros { get; set; }
        public ContentManagementFlagAction Metadata { get; set; }
        public ContentManagementFlagAction ReviewComments { get; set; }
    }
}