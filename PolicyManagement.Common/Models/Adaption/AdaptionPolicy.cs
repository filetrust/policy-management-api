﻿using Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags;
using Glasswall.PolicyManagement.Common.Models.Ncfs;

namespace Glasswall.PolicyManagement.Common.Models.Adaption
{
    public class AdaptionPolicy
    {
        public ContentManagementFlags ContentManagementFlags { get; set; }
        public string ErrorReportTemplate { get; set; }
        public NcfsRoute NcfsRoute { get; set; }
        public NcfsActions NcfsActions { get; set; }
    }
}