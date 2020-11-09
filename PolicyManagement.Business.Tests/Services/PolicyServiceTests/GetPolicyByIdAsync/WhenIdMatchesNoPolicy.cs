using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.GetPolicyByIdAsync
{
    [TestFixture]
    public class WhenIdMatchesNoPolicy : PolicyServiceTestBase
    {
        private PolicyModel _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();
            
            ExpectedModel.Id = Guid.NewGuid();

            _fileShare.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _output = await ClassInTest.GetPolicyByIdAsync(ExpectedModel.Id, Token);
        }

        [Test]
        public void Policy_Is_Null()
        {
            Assert.That(_output, Is.Null);
        }
    }
}
