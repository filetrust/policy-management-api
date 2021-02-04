using System.ComponentModel.DataAnnotations;

namespace Glasswall.PolicyManagement.Common.Models
{
    public class GetHistoricPoliciesRequestModel
    {
        [Required]
        public PaginationModel Pagination { get; set; }
    }

    public class PaginationModel
    {
        [Required]
        public int? ZeroBasedIndex { get; set; }

        [Required]
        public int? PageSize { get; set; }
    }
}