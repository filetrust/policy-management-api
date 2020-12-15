using Glasswall.PolicyManagement.Business.BackgroundServices;
using Glasswall.PolicyManagement.Common.BackgroundServices;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using TestCommon;

namespace PolicyManagement.Business.Tests.BackgroundServices.NcfsPolicyDistributerTests.SyncPoliciesAsync
{
    public abstract class NcfsPolicyDistributerTestBase : UnitTestBase<IPolicySynchronizer>
    {
        protected Mock<ILogger<NcfsPolicyDistributer>> Logger;
        protected Mock<IPolicyManagementApiConfiguration> Configuration;
        protected Mock<IPolicyService> Service;
        protected Mock<IPolicyDistributer> Distributer;

        protected CancellationToken Token;

        public void SharedSetup()
        {
            Logger = new Mock<ILogger<NcfsPolicyDistributer>>();
            Configuration = new Mock<IPolicyManagementApiConfiguration>();
            Service = new Mock<IPolicyService>();
            Distributer = new Mock<IPolicyDistributer>();

            Token = new CancellationToken(false);

            ClassInTest = new NcfsPolicyDistributer(Logger.Object, Configuration.Object, Service.Object, Distributer.Object);
        }
    }
}