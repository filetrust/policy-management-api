using System;
using Glasswall.PolicyManagement.Common.Models.Adaption;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Models.Ncfs;

namespace Glasswall.PolicyManagement.Common.Models
{
    public class PolicyModel
    {
        public Guid Id { get; set; }

        public PolicyState PolicyState { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string UpdatedBy { get; set; }

        public AdaptionPolicy AdaptionPolicy { get; set; }

        public NcfsPolicy NcfsPolicy { get; set; }
    }
}