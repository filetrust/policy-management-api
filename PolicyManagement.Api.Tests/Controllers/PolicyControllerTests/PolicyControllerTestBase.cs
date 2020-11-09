using System.Threading;
using Glasswall.PolicyManagement.Api.Controllers;
using Glasswall.PolicyManagement.Common.Services;
using Moq;
using TestCommon;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests
{
    public class PolicyControllerTestBase : UnitTestBase<PolicyController>
    {
        protected Mock<IPolicyService> Service;
        protected Mock<IPolicyDistributer> Distributer;
        protected CancellationToken MockCancellationToken;

        public void SharedSetup()
        {
            Service = new Mock<IPolicyService>();
            Distributer = new Mock<IPolicyDistributer>();

            MockCancellationToken = new CancellationToken(false);

            ClassInTest = new PolicyController(Distributer.Object, Service.Object);
        }
    }
}
