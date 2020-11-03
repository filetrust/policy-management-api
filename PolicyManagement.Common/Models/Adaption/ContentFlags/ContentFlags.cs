namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    public class ContentFlags
    {
        public PdfContentFlags PdfContentManagement { get; set; }
        public WordContentFlags WordContentManagement { get; set; }
        public ExcelContentFlags ExcelContentManagement { get; set; }
        public PowerPointContentFlags PowerPointContentManagement { get; set; }
    }
}