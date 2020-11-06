using System;
using Glasswall.PolicyManagement.Common.Models.Adaption;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Models.Ncfs;

namespace Glasswall.PolicyManagement.Common.Models
{
    public class PolicyModel
    {
        public Guid Id { get; set; }

        public PolicyType PolicyType { get; set; }

        public DateTimeOffset? Published { get; set; }

        public DateTimeOffset LastEdited { get; set; }

        public DateTimeOffset Created { get; set; }

        public string UpdatedBy { get; set; }

        public AdaptionPolicy AdaptionPolicy { get; set; }

        public NcfsPolicy NcfsPolicy { get; set; }

        public PolicyModel ToDraft()
        {
            return new PolicyModel
            {
                Id = Guid.NewGuid(),
                PolicyType = PolicyType.Draft,
                LastEdited = DateTimeOffset.UtcNow,
                UpdatedBy = null,
                AdaptionPolicy = AdaptionPolicy,
                NcfsPolicy = NcfsPolicy,
                Created = DateTimeOffset.UtcNow,
                Published = null
            };
        }
    }
}