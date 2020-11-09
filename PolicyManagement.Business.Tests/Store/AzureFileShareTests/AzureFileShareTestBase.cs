using System.Collections.Generic;
using System.Threading;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Glasswall.PolicyManagement.Business.Store;
using Moq;
using TestCommon;

namespace PolicyManagement.Business.Tests.Store.AzureFileShareTests
{
    public class AzureFileShareTestBase : UnitTestBase<AzureFileShare>
    {
        protected Mock<ShareClient> ShareClient;

        protected void SharedSetup()
        {
            ShareClient = new Mock<ShareClient>();
            ClassInTest = new AzureFileShare(ShareClient.Object);
        }

        protected static Mock<AsyncPageable<ShareFileItem>> MockPageable(IEnumerable<ShareFileItem> itemsInOrderOfProcessing)
        {
            var mockedPagable = new Mock<AsyncPageable<ShareFileItem>>();
            mockedPagable.Setup(s => s.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(itemsInOrderOfProcessing.AsAsyncEnumerator());
            return mockedPagable;
        }
    }
}