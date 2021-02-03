using System;
using Glasswall.PolicyManagement.Common.Models.Adaption;
using Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Models.Ncfs;
using System.Diagnostics.CodeAnalysis;

namespace Glasswall.PolicyManagement.Common.Models
{
    public class PolicyModel
    {
        public Guid Id { get; set; }

        public PolicyType PolicyType { get; set; }

        [ExcludeFromCodeCoverage]
        public DateTimeOffset? Published { get; set; }

        public DateTimeOffset LastEdited { get; set; }

        [ExcludeFromCodeCoverage]
        public DateTimeOffset Created { get; set; }

        [ExcludeFromCodeCoverage]
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

        public static PolicyModel Default(PolicyType type)
        {
            return new PolicyModel
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow,
                LastEdited = DateTimeOffset.MinValue,
                PolicyType = type,
                Published = type == PolicyType.Current ? (DateTimeOffset?)DateTimeOffset.UtcNow : null,
                UpdatedBy = "",
                NcfsPolicy = new NcfsPolicy
                {
                    NcfsActions = new NcfsActions
                    {
                        GlasswallBlockedFilesAction = NcfsOption.Relay,
                        UnprocessableFileTypeAction = NcfsOption.Relay
                    }
                },
                AdaptionPolicy = new AdaptionPolicy
                {
                    ErrorReportTemplate = "",
                    NcfsActions = new NcfsActions
                    {
                        GlasswallBlockedFilesAction = NcfsOption.Block,
                        UnprocessableFileTypeAction = NcfsOption.Block
                    },
                    ContentManagementFlags = new ContentManagementFlags
                    {
                        ExcelContentManagement = new ExcelContentManagement
                        {
                            DynamicDataExchange = ContentManagementFlagAction.Sanitise,
                            EmbeddedFiles = ContentManagementFlagAction.Sanitise,
                            EmbeddedImages = ContentManagementFlagAction.Sanitise,
                            ExternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            InternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            Macros = ContentManagementFlagAction.Sanitise,
                            Metadata = ContentManagementFlagAction.Sanitise,
                            ReviewComments = ContentManagementFlagAction.Sanitise
                        },
                        PdfContentManagement = new PdfContentManagement
                        {
                            EmbeddedFiles = ContentManagementFlagAction.Sanitise,
                            EmbeddedImages = ContentManagementFlagAction.Sanitise,
                            ExternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            InternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            Metadata = ContentManagementFlagAction.Sanitise,
                            Acroform = ContentManagementFlagAction.Sanitise,
                            ActionsAll = ContentManagementFlagAction.Sanitise,
                            Javascript = ContentManagementFlagAction.Sanitise
                        },
                        PowerPointContentManagement = new PowerPointContentManagement
                        {
                            EmbeddedImages = ContentManagementFlagAction.Sanitise,
                            ExternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            InternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            Metadata = ContentManagementFlagAction.Sanitise,
                            EmbeddedFiles = ContentManagementFlagAction.Sanitise,
                            Macros = ContentManagementFlagAction.Sanitise,
                            ReviewComments = ContentManagementFlagAction.Sanitise
                        },
                        WordContentManagement = new WordContentManagement
                        {
                            ExternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            InternalHyperlinks = ContentManagementFlagAction.Sanitise,
                            Metadata = ContentManagementFlagAction.Sanitise,
                            EmbeddedFiles = ContentManagementFlagAction.Sanitise,
                            EmbeddedImages = ContentManagementFlagAction.Sanitise,
                            Macros = ContentManagementFlagAction.Sanitise,
                            ReviewComments = ContentManagementFlagAction.Sanitise,
                            DynamicDataExchange = ContentManagementFlagAction.Sanitise
                        }
                    }
                }
            };
        }
    }
}