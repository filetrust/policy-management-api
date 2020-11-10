using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
using TestCommon;

namespace PolicyManagement.Business.Tests.Services.PolicyDistributerTests.Distribute
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

            await ClassInTest.Distribute(_input = new PolicyModel
            {
                Id = Guid.NewGuid(),
                AdaptionPolicy = new AdaptionPolicy { ContentManagementFlags = new ContentFlags() },
                NcfsPolicy = new NcfsPolicy
                {
                    Options = new NcfsOptions
                    {
                        GlasswallBlockedFiles = NcfsOption.Block,
                        UnProcessableFileTypes = NcfsOption.Refer
                    },
                    Routes = new[]
                    {
                        new NcfsRoute(), 
                    }
                }
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
                    ContentManagementFlags = _input.AdaptionPolicy?.ContentManagementFlags,
                    UnprocessableFileTypeAction = _input.NcfsPolicy?.Options?.UnProcessableFileTypes,
                    GlasswallBlockedFilesAction = _input.NcfsPolicy?.Options?.GlasswallBlockedFiles,
                    NcfsRoutingUrl = _input.NcfsPolicy?.Routes?.FirstOrDefault()?.ApiUrl
                })).Times(1);

            _httpTest.ShouldHaveCalled("http://endpoint2:401/api/v1/policy")
                .With(x => x.HttpRequestMessage.Method == HttpMethod.Put)
                .With(x => x.RequestBody == Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    PolicyId = _input.Id,
                    ContentManagementFlags = _input.AdaptionPolicy?.ContentManagementFlags,
                    UnprocessableFileTypeAction = _input.NcfsPolicy?.Options?.UnProcessableFileTypes,
                    GlasswallBlockedFilesAction = _input.NcfsPolicy?.Options?.GlasswallBlockedFiles,
                    NcfsRoutingUrl = _input.NcfsPolicy?.Routes?.FirstOrDefault()?.ApiUrl
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
