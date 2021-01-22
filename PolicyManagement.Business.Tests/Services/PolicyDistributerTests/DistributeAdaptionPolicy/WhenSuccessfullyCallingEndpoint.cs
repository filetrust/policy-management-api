using Flurl.Http.Testing;
using Glasswall.PolicyManagement.Business.Services;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Models;
using Glasswall.PolicyManagement.Common.Models.Adaption;
using Glasswall.PolicyManagement.Common.Models.Adaption.ContentFlags;
using Glasswall.PolicyManagement.Common.Models.Enums;
using Glasswall.PolicyManagement.Common.Models.Ncfs;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TestCommon;

namespace PolicyManagement.Business.Tests.Services.PolicyDistributerTests.DistributeAdaptionPolicy
{
    [TestFixture]
    public class WhenSuccessfullyCallingEndpoint : UnitTestBase<PolicyDistributer>
    {
        private Mock<ILogger<PolicyDistributer>> _logger;
        private Mock<IPolicyManagementApiConfiguration> _configuration;
        private PolicyModel _input;
        private CancellationToken _token;

        private HttpTest _httpTest;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _logger = new Mock<ILogger<PolicyDistributer>>();
            _configuration = new Mock<IPolicyManagementApiConfiguration>();

            ClassInTest = new PolicyDistributer(_logger.Object, _configuration.Object);

            _configuration.Setup(s => s.PolicyUpdateServiceEndpointCsv)
                .Returns("https://endpoint1:3001,http://endpoint2:401");

            _httpTest = new HttpTest();

            _httpTest.RespondWith("body");
            _httpTest.RespondWith("body");
            _httpTest.RespondWith("body");
            _httpTest.RespondWith("body");

            await ClassInTest.DistributeAdaptionPolicy(_input = new PolicyModel
            {
                Id = Guid.NewGuid(),
                AdaptionPolicy = new AdaptionPolicy
                {
                    ContentManagementFlags = new ContentManagementFlags(), 
                    ErrorReportTemplate = "This banana is for you",
                    NcfsActions = new NcfsActions
                    {
                        GlasswallBlockedFilesAction = NcfsOption.Block,
                        UnprocessableFileTypeAction = NcfsOption.Relay
                    },
                    NcfsRoute = new NcfsRoute
                    {
                        NcfsRoutingUrl = "routey@mc.route.face"
                    }
                },
            }, _token = new CancellationToken());

        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _httpTest.Dispose();
        }

        [Test]
        public void Each_Endpoint_Invokes_Put()
        {
            _httpTest.ShouldHaveCalled("https://endpoint1:3001/api/v1/policy")
                .With(x => x.HttpRequestMessage.Method == HttpMethod.Put)
                .With(x => x.RequestBody == Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    PolicyId = _input.Id,
                    _input.AdaptionPolicy?.ContentManagementFlags,
                    _input.AdaptionPolicy?.NcfsActions?.UnprocessableFileTypeAction,
                    _input.AdaptionPolicy?.NcfsActions?.GlasswallBlockedFilesAction,
                    NcfsRoutingUrl = _input.AdaptionPolicy?.NcfsRoute?.NcfsRoutingUrl ?? "https://ncfs-policy-update-service.icap-ncfs.svc.cluster.local",
                    RebuildReportMessage = _input.AdaptionPolicy?.ErrorReportTemplate ?? "File could not be rebuilt",
                    ArchivePasswordProtectedReportMessage = _input.AdaptionPolicy?.ArchivePasswordProtectedReportMessage ?? "Archive is password protected and could not be rebuilt",
                    ArchiveErrorReportMessage = _input.AdaptionPolicy?.ArchiveErrorReportMessage ?? "Archive contains an error and could not be rebuilt"
                })).Times(1);

            _httpTest.ShouldHaveCalled("http://endpoint2:401/api/v1/policy")
                .With(x => x.HttpRequestMessage.Method == HttpMethod.Put)
                .With(x => x.RequestBody == Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    PolicyId = _input.Id,
                    _input.AdaptionPolicy?.ContentManagementFlags,
                    _input.AdaptionPolicy?.NcfsActions?.UnprocessableFileTypeAction,
                    _input.AdaptionPolicy?.NcfsActions?.GlasswallBlockedFilesAction,
                    NcfsRoutingUrl = _input.AdaptionPolicy?.NcfsRoute?.NcfsRoutingUrl ?? "https://ncfs-policy-update-service.icap-ncfs.svc.cluster.local",
                    RebuildReportMessage = _input.AdaptionPolicy?.ErrorReportTemplate ?? "File could not be rebuilt",
                    ArchivePasswordProtectedReportMessage = _input.AdaptionPolicy?.ArchivePasswordProtectedReportMessage ?? "Archive is password protected and could not be rebuilt",
                    ArchiveErrorReportMessage = _input.AdaptionPolicy?.ArchiveErrorReportMessage ?? "Archive contains an error and could not be rebuilt"
                })).Times(1);

            Assert.That(_httpTest.CallLog.Count, Is.EqualTo(4));
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<PolicyDistributer>();
        }
    }
}
