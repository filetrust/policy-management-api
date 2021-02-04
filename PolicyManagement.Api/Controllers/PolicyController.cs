using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Glasswall.PolicyManagement.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyDistributer _policyDistributer;
        private readonly IPolicyService _policyService;

        public PolicyController(
            IPolicyDistributer policyDistributer,
            IPolicyService policyService)
        {
            _policyDistributer = policyDistributer ?? throw new ArgumentNullException(nameof(policyDistributer));
            _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
        }

        [HttpGet("draft")]
        public async Task<IActionResult> GetDraftPolicy(CancellationToken cancellationToken)
        {
            var policy = await _policyService.GetDraftAsync(cancellationToken);

            return Ok(policy ?? PolicyModel.Default(PolicyType.Draft));
        }

        [HttpPut("draft")]
        public async Task<IActionResult> SavePolicy([FromBody] PolicyModel policyModel, CancellationToken cancellationToken)
        {
            await _policyService.SaveAsDraftAsync(policyModel, cancellationToken);

            return Ok();
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentPolicy(CancellationToken cancellationToken)
        {
            var policy = await _policyService.GetCurrentAsync(cancellationToken);

            return Ok(policy ?? PolicyModel.Default(PolicyType.Current));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoricPolicies(CancellationToken cancellationToken)
        {
            var policies = new List<PolicyModel>();

            await foreach (var policy in _policyService.GetHistoricalPoliciesAsync(cancellationToken))
                policies.Add(policy);

            if (!policies.Any())
                return NoContent();

            return Ok(new HistoryResponse
            {
                Policies = policies,
                PoliciesCount = policies.Count
            });
        }

        [HttpPost("history")]
        public async Task<IActionResult> GetHistoricPolicies([FromBody][Required]GetHistoricPoliciesRequestModel model, CancellationToken cancellationToken)
        {
            var policies = new List<PolicyModel>();

            var policyIndex = 0;

            var startCollectingIndex = model.Pagination.ZeroBasedIndex * model.Pagination.PageSize; // e.g 0, 25, 50
            var stopCollectingIndex = startCollectingIndex + model.Pagination.PageSize; // e.g 25, 50, 75

            await foreach (var policy in _policyService.GetHistoricalPoliciesAsync(cancellationToken))
            {
                if (policyIndex >= startCollectingIndex && policyIndex < stopCollectingIndex) policies.Add(policy);
                
                policyIndex++;

                if (policyIndex >= stopCollectingIndex) break;
            }

            if (!policies.Any())
                return NoContent();

            return Ok(new HistoryResponse
            {
                Policies = policies,
                PoliciesCount = policies.Count
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPolicy([FromQuery]Guid id, CancellationToken cancellationToken)
        {
            var policy = await _policyService.GetPolicyByIdAsync(id, cancellationToken);

            if (policy == null)
                return NoContent();

            return Ok(policy);
        }

        [HttpPut("publish")]
        public async Task<IActionResult> PublishDraft([FromQuery]Guid id, CancellationToken cancellationToken)
        {
            await _policyService.PublishAsync(id, cancellationToken);

            return Ok();
        }

        [HttpPut("current/distribute-adaption")]
        public async Task<IActionResult> DistributeCurrent(CancellationToken cancellationToken)
        {
            var currentPolicy = await _policyService.GetCurrentAsync(cancellationToken);

            await _policyDistributer.DistributeAdaptionPolicy(currentPolicy, cancellationToken);

            return Ok();
        }

        [HttpPut("current/distribute-ncfs")]
        public async Task<IActionResult> DistributeNcfsPolicy(CancellationToken cancellationToken)
        {
            var currentPolicy = await _policyService.GetCurrentAsync(cancellationToken);

            await _policyDistributer.DistributeNcfsPolicy(currentPolicy, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePolicy([FromQuery]Guid id, CancellationToken cancellationToken)
        {
            await _policyService.DeleteAsync(id, cancellationToken);

            return Ok();
        }
    }
}