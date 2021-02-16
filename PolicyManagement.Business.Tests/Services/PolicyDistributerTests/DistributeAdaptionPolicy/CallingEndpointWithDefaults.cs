using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Glasswall.PolicyManagement.Business.Services;
using Glasswall.PolicyManagement.Common.Configuration;
using Glasswall.PolicyManagement.Common.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Services.PolicyDistributerTests.DistributeAdaptionPolicy
{
    [TestFixture]
    public class CallingEndpointWithDefaults : UnitTestBase<PolicyDistributer>
    {
        private Mock<ILogger<PolicyDistributer>> _logger;
        private Mock<IPolicyManagementApiConfiguration> _configuration;
        private PolicyModel _input;

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
                Id = Guid.NewGuid()
            }, new CancellationToken());

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
                    NcfsRoutingUrl = "https://ncfs-reference-service.icap-ncfs.svc.cluster.local",
                    RebuildReportMessage = "File could not be rebuilt",
                    ArchivePasswordProtectedReportMessage = "Archive is password protected and could not be rebuilt",
                    ArchiveErrorReportMessage = "Archive contains an error and could not be rebuilt"
                })).Times(1);

            _httpTest.ShouldHaveCalled("http://endpoint2:401/api/v1/policy")
                .With(x => x.HttpRequestMessage.Method == HttpMethod.Put)
                .With(x => x.RequestBody == Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    PolicyId = _input.Id,
                    _input.AdaptionPolicy?.ContentManagementFlags,
                    _input.AdaptionPolicy?.NcfsActions?.UnprocessableFileTypeAction,
                    _input.AdaptionPolicy?.NcfsActions?.GlasswallBlockedFilesAction,
                    NcfsRoutingUrl = "https://ncfs-reference-service.icap-ncfs.svc.cluster.local",
                    RebuildReportMessage = "File could not be rebuilt",
                    ArchivePasswordProtectedReportMessage = "Archive is password protected and could not be rebuilt",
                    ArchiveErrorReportMessage = "Archive contains an error and could not be rebuilt"
                })).Times(1);

            Assert.That(_httpTest.CallLog.Count, Is.EqualTo(4));
        }
    }
}
