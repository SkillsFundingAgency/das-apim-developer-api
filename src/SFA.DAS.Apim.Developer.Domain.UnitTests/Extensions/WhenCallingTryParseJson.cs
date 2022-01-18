using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Extensions;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Extensions
{
    public class WhenCallingTryParseJson
    {
        [TestCase("test@test.com", false)]
        [TestCase("asdfasdfasdf", false)]
        [TestCase("{}", true)]
        [TestCase(@"{""testProperty"":""test value""}", true)]
        [TestCase(@"{""wrongPropertyName"":""test value""}", false)]
        public void Then_Processes_Correct_Response(string rawJson, bool expectedResult)
        {
            rawJson.TryParseJson(out TestJson json).Should().Be(expectedResult);
            if (expectedResult)
            {
                json.Should().BeEquivalentTo(JsonConvert.DeserializeObject<TestJson>(rawJson));
            }
        }
    }

    public class TestJson
    {
        public string TestProperty { get; set; }
    }
}