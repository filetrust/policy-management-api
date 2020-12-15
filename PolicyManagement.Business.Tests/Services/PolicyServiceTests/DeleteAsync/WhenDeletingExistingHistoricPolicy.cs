using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.DeleteAsync
{
    [TestFixture]
    public class WhenDeletingExistingHistoricPolicy : PolicyServiceTestBase
    {
        private Guid _input;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _fileShare.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await ClassInTest.DeleteAsync(_input = Guid.NewGuid(), Token);
        }

        [Test]
        public void Policy_Existence_Checked()
        {
            _fileShare.Verify(s => s.ExistsAsync(It.Is<string>(f => f == $"historical/{_input}"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }

        [Test]
        public void Policy_Is_Deleted()
        {
            _fileShare.Verify(s => s.DeleteAsync(It.Is<string>(f => f == $"historical/{_input}"), It.Is<CancellationToken>(f => f == Token)), Times.Once);
        }
    }
}
