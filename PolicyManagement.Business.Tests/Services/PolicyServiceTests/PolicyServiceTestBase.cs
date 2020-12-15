using System.Threading;
using Glasswall.PolicyManagement.Business.Services;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Serialisation;
using Glasswall.PolicyManagement.Common.Store;
using Microsoft.Extensions.Logging;
using Moq;
using TestCommon;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests
{
    public class PolicyServiceTestBase : UnitTestBase<PolicyService>
    {
        protected Mock<IFileStore> _fileShare;
        protected Mock<ILogger<PolicyService>> _logger;
        protected Mock<IJsonSerialiser> _serializer;
        protected PolicyModel ExpectedModel;
        protected CancellationToken Token;

        protected void SharedSetup()
        {
            _fileShare = new Mock<IFileStore>();
            _logger = new Mock<ILogger<PolicyService>>();
            _serializer = new Mock<IJsonSerialiser>();

            ExpectedModel = new PolicyModel();
            Token = new CancellationToken(false);

            ClassInTest = new PolicyService(_fileShare.Object, _logger.Object, _serializer.Object);
        }
    }
}