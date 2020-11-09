using System;
using System.Linq;
using Glasswall.PolicyManagement.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Api.Tests.Controllers.PolicyControllerTests.Constructor
{
    [TestFixture]
    public class WhenConstructing : UnitTestBase<PolicyController>
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<PolicyController>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<PolicyController>();
        }

        [Test]
        public void Constructed_Class_Has_Correct_Attributes()
        {
            var attributes = typeof(PolicyController).GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(2).Items);
            Assert.That(attributes[0], Is.InstanceOf<ApiControllerAttribute>());
            Assert.That(attributes[1], Is.InstanceOf<RouteAttribute>().With.Property(nameof(RouteAttribute.Template)).EqualTo("api/v1/[controller]"));
        }

        [Test]
        [TestCase(nameof(PolicyController.GetDraftPolicy), typeof(HttpGetAttribute), "draft")]
        [TestCase(nameof(PolicyController.SavePolicy), typeof(HttpPutAttribute), "draft")]
        [TestCase(nameof(PolicyController.GetCurrentPolicy), typeof(HttpGetAttribute), "current")]
        [TestCase(nameof(PolicyController.GetHistoricPolicies), typeof(HttpGetAttribute), "history")]
        [TestCase(nameof(PolicyController.GetPolicy), typeof(HttpGetAttribute), null)]
        [TestCase(nameof(PolicyController.PublishDraft), typeof(HttpPutAttribute), "publish")]
        [TestCase(nameof(PolicyController.DistributeCurrent), typeof(HttpPutAttribute), "current/distribute")]
        [TestCase(nameof(PolicyController.DeletePolicy), typeof(HttpDeleteAttribute), null)]
        public void Constructed_Class_Method_Has_Correct_Attributes(string method, Type verb, string expectedRoute)
        {
            var attributes = typeof(PolicyController).GetMethod(method)?.GetCustomAttributes(false);

            Assert.That(attributes, Is.Not.Null);

            Assert.That(attributes.Last(),
                Is.InstanceOf(verb).With.Property(nameof(HttpMethodAttribute.Template)).EqualTo(expectedRoute));
        }
    }
}
