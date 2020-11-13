using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Glasswall.PolicyManagement.Api.Controllers;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Serialisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PolicyManagement.Api.Function;

[assembly: FunctionsStartup(typeof(Startup))]

namespace PolicyManagement.Api.Function
{
    [ExcludeFromCodeCoverage]
    public class PolicyControllerFunction
    {
        private readonly IJsonSerialiser _jsonSerialiser;
        private readonly PolicyController _controller;

        public PolicyControllerFunction(
            IJsonSerialiser jsonSerialiser,
            PolicyController controller)
        {
            _jsonSerialiser = jsonSerialiser ?? throw new ArgumentNullException(nameof(jsonSerialiser));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        [FunctionName("policy")]
        public async Task<IActionResult> ExecutePolicyController(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete", Route = "v1/{*restOfPath}")] HttpRequest req,
            ILogger log,
            string restOfPath,
            CancellationToken cancellationToken)
        {
            try
            {
                var method = req.Method.ToLower();

                if (restOfPath == "policy")
                {
                    if (method == "get")
                        return await _controller.GetPolicy(Guid.Parse(req.Query.First().Value), cancellationToken);
                    if (method == "delete")
                        return await _controller.DeletePolicy(Guid.Parse(req.Query.First().Value), cancellationToken);
                }

                if (restOfPath == "policy/draft")
                {
                    if (method == "get") return await _controller.GetDraftPolicy(cancellationToken);
                    if (method == "put")
                    {
                        await using (var ms = new MemoryStream())
                        {
                            await req.Body.CopyToAsync(ms, cancellationToken);
                            var json = Encoding.UTF8.GetString(ms.ToArray());
                            var policy = Newtonsoft.Json.JsonConvert.DeserializeObject<PolicyModel>(json);
                            return await _controller.SavePolicy(policy, cancellationToken);
                        }
                    }
                }

                if (restOfPath == "policy/current")
                    if (method == "get")
                        return await _controller.GetCurrentPolicy(cancellationToken);

                if (restOfPath == "policy/history")
                    if (method == "get")
                        return await _controller.GetHistoricPolicies(cancellationToken);

                if (restOfPath == "policy/publish")
                    if (method == "put")
                        return await _controller.PublishDraft(Guid.Parse(req.Query.First().Value), cancellationToken);

                if (restOfPath == "policy/current/distribute-adaption")
                    if (method == "put")
                        return await _controller.DistributeCurrent(cancellationToken);

                if (restOfPath == "policy/current/distribute-ncfs")
                    if (method == "put")
                        return await _controller.DistributeNcfsPolicy(cancellationToken);

                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
