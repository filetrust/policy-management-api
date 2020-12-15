using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.SaveAsDraftAsync
{
    [TestFixture]
    public class WhenPolicyIsValid : PolicyServiceTestBase
    {
        private string _jsonFile;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            ExpectedModel.LastEdited = DateTime.MinValue; // should get updated

            _serializer.Setup(s => s.Serialize(It.IsAny<PolicyModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_jsonFile = "majson");

            await ClassInTest.SaveAsDraftAsync(ExpectedModel, CancellationToken.None);
        }

        [Test]
        public void JsonSerializer_Is_Called()
        {
            _serializer.Verify(x => x.Serialize<PolicyModel>(
                It.Is<PolicyModel>(model => 
                    model == ExpectedModel &&
                    model.LastEdited != DateTime.MinValue
                    ),
                It.Is<CancellationToken>(f => f == Token)
            ), Times.Once);
            _serializer.VerifyNoOtherCalls();
        }

        [Test]
        public void UTF8_String_Uploaded()
        {
            _fileShare.Verify(x => x.WriteAsync(It.Is<string>(f => f == "draft/policy.json"),
                    It.Is<byte[]>(f => Encoding.UTF8.GetString(f) == _jsonFile),
                    It.Is<CancellationToken>(f => f == Token)),
                Times.Once);
            _fileShare.VerifyNoOtherCalls();
        }
    }
}
