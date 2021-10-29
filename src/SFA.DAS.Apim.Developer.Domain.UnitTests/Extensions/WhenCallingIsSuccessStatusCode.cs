using System.Net;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Extensions;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Extensions
{
    public class WhenCallingIsSuccessStatusCode
    {
        [TestCase(100, false)]
        [TestCase(199, false)]
        [TestCase(200, true)]
        [TestCase(299, true)]
        [TestCase(300, false)]
        [TestCase(400, false)]
        [TestCase(500, false)]
        public void Then_Processes_Correct_Response(HttpStatusCode statusCode, bool expectedResult)
        {
            statusCode.IsSuccessStatusCode().Should().Be(expectedResult);
        }
    }
}