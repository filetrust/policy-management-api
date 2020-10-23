using System;
using System.Linq;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Glasswall.PolicyManagement.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        [HttpGet("draft")]
        public async Task<IActionResult> GetDraftPolicy()
        {
            var policy = await _policyService.GetDraftAsync();

            if (policy == null)
                return NoContent();

            return Ok(policy);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentPolicy()
        {
            var policy = await _policyService.GetCurrentAsync();

            if (policy == null)
                return NoContent();

            return Ok(policy);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoricPolicies()
        {
            var policies = await _policyService.GetHistoricalPoliciesAsync();

            if (!policies.Any())
                return NoContent();

            return Ok(policies);
        }

        [HttpGet]
        public async Task<IActionResult> GetPolicy([FromQuery]Guid id)
        {
            var policy = await _policyService.GetPolicyByIdAsync(id);

            return Ok(policy);
        }

        [HttpPost("publish")]
        public async Task<IActionResult> PublishDraft()
        {
            await _policyService.PublishAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> SavePolicy([FromBody]PolicyModel policyModel)
        {
            await _policyService.SaveAsync(policyModel);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePolicy([FromQuery]Guid id)
        {
            await _policyService.DeleteAsync(id);

            return Ok();
        }
    }
}