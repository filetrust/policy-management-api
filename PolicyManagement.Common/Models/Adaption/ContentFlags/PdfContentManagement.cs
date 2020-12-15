using Glasswall.PolicyManagement.Common.Models.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    [ExcludeFromCodeCoverage]
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