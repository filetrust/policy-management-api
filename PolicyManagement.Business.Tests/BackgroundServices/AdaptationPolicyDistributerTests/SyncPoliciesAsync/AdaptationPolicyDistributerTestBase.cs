using System.Threading;
using Glasswall.PolicyManagement.Business.BackgroundServices;
using Glasswall.PolicyManagement.Common.BackgroundServices;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Services;
using Microsoft.Extensions.Logging;
using Moq;
using TestCommon;

namespace PolicyManagement.Business.Tests.BackgroundServices.AdaptationPolicyDistributerTests.SyncPoliciesAsync
{
    public abstract class AdaptationPolicyDistributerTestBase : UnitTestBase<IPolicySynchronizer>
    {
        protected Mock<ILogger<AdaptationPolicyDistributer>> Logger;
        protected Mock<IPolicyManagementApiConfiguration> Configuration;
        protected Mock<IPolicyService> Service;
        protected Mock<IPolicyDistributer> Distributer;

        protected CancellationToken Token;

        public void SharedSetup()
        {
            Logger = new Mock<ILogger<AdaptationPolicyDistributer>>();
            Configuration = new Mock<IPolicyManagementApiConfiguration>();
            Service = new Mock<IPolicyService>();
            Distributer = new Mock<IPolicyDistributer>();

            Token = new CancellationToken(false);

            ClassInTest = new AdaptationPolicyDistributer(Logger.Object, Configuration.Object, Service.Object, Distributer.Object);
        }
    }
}