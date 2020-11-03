using Glasswall.PolicyManagement.Common.Models.Enums;

namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    public class PdfContentFlags
    {
        public ContentFlagAction Acroform { get; set; }
        public ContentFlagAction ActionsAll { get; set; }
        public ContentFlagAction EmbeddedFiles { get; set; }
        public ContentFlagAction EmbeddedImages { get; set; }
        public ContentFlagAction ExternalHyperlinks { get; set; }
        public ContentFlagAction InternalHyperlinks { get; set; }
        public ContentFlagAction Javascript { get; set; }
        public ContentFlagAction Metadata { get; set; }
    }
}