using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Business.Serialisation;
using Glasswall.PolicyManagement.Common.Serialisation;
using NUnit.Framework;
using TestCommon;

namespace PolicyManagement.Business.Tests.Serialisation.JsonSerialiserTests
{
    [TestFixture]
    public class JsonSerialiserTests : UnitTestBase<IJsonSerialiser>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ClassInTest = new JsonSerialiser();
        }

        [Test]
        public void Null_Input_Throws_For_Serialise()
        {
            Assert.That(() => ClassInTest.Serialize<object>(null, CancellationToken.None), ThrowsArgumentNullException("input"));
        }

        [Test]
        public void Null_Input_Throws_For_Deserialise()
        {
            Assert.That(() => ClassInTest.Deserialize<object>(null, CancellationToken.None), ThrowsArgumentNullException("input"));
        }

        [Test]
        public async Task Serialise_Is_Correct()
        {
            var str = await ClassInTest.Serialize(new 
            {
                TestProp = "Test"
            }, CancellationToken.None);

            Assert.That(str, Is.EqualTo("{\"TestProp\":\"Test\"}"));
        }

        [Test]
        public async Task Deserialise_String_Is_Correct()
        {
            await using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("{\"TestProp\":\"Test\"}")))
            {
                var obj = await ClassInTest.Deserialize<Dictionary<string, string>>(ms, CancellationToken.None);
                Assert.That(obj["TestProp"], Is.EqualTo("Test"));
            }
        }
    }
}