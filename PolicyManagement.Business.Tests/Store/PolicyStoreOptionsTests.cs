using System;
using Glasswall.PolicyManagement.Business.Store;
using NUnit.Framework;
using Polly;

namespace PolicyManagement.Business.Tests.Store
{
    [TestFixture]
    public class PolicyStoreOptionsTests
    {
        [Test]
        public void Constructs_With_Mocks()
        {
            PolicyStoreOptions e = null;
            AsyncPolicy b = null;
            Assert.That(() => e = new PolicyStoreOptions("root", b = Policy.Handle<Exception>(e => true).RetryAsync()), Throws.Nothing);

            Assert.That(e.RetryPolicy, Is.EqualTo(b));
            Assert.That(e.RootPath, Is.EqualTo("root"));
        }

        [Test]
        public void Throws_With_Null()
        {
            Assert.That(() => new PolicyStoreOptions(null, Policy.Handle<Exception>(e => true).RetryAsync()), Throws.Exception);
            Assert.That(() => new PolicyStoreOptions("root", null), Throws.Exception);
        }
    }
}
