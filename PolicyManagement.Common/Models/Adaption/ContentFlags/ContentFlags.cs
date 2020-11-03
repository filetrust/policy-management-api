namespace Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags
{
    public class ContentFlags
    {
        public PdfContentFlags PdfContentFlags { get; set; }
        public WordContentFlags WordContentFlags { get; set; }
        public ExcelContentFlags ExcelContentFlags { get; set; }
        public PowerPointContentFlags PowerPointContentFlags { get; set; }
    }
}