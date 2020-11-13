namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    public class ContentManagementFlags
    {
        public PdfContentManagement PdfContentManagement { get; set; }
        public WordContentManagement WordContentManagement { get; set; }
        public ExcelContentManagement ExcelContentManagement { get; set; }
        public PowerPointContentManagement PowerPointContentManagement { get; set; }
    }
}