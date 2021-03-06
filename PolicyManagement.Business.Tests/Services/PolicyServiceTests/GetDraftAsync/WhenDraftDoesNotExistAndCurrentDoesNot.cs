﻿using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Models;
using Moq;
using NUnit.Framework;

namespace PolicyManagement.Business.Tests.Services.PolicyServiceTests.GetDraftAsync
{
    [TestFixture]
    public class WhenDraftDoesNotExistAndCurrentDoesNot : PolicyServiceTestBase
    {
        private PolicyModel _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();
            
            _output = await ClassInTest.GetDraftAsync(Token);
        }

        [Test]
        public void Policy_Existence_Checked()
        {
            _fileShare.Verify(s => s.ExistsAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)));
        }

        [Test]
        public void Draft_Policy_Not_Downloaded()
        {
            _fileShare.Verify(s => s.ReadAsync(It.Is<string>(f => f == "draft/policy.json"), It.Is<CancellationToken>(f => f == Token)), Times.Never);
        }

        [Test]
        public void NullPolicy_Is_Returned()
        {
            Assert.That(_output, Is.Null);
        }
    }
}
