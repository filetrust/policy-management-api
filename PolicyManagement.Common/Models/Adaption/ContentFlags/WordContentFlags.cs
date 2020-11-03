using Glasswall.PolicyManagement.Common.Models.Enums;

namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    public class WordContentFlags
    {
        public ContentFlagAction DynamicDataExchange { get; set; }
        public ContentFlagAction EmbeddedFiles { get; set; }
        public ContentFlagAction EmbeddedImages { get; set; }
        public ContentFlagAction ExternalHyperlinks { get; set; }
        public ContentFlagAction InternalHyperlinks { get; set; }
        public ContentFlagAction Macros { get; set; }
        public ContentFlagAction Metadata { get; set; }
        public ContentFlagAction ReviewComments { get; set; }
    }
}