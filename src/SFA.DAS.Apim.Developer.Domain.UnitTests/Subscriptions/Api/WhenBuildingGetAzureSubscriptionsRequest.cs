using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingGetAzureSubscriptionsRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            var actual = new GetAzureSubscriptionsRequest();

            actual.GetUrl.Should().Be("subscriptions?api-version=2020-01-01");
        }
    }
}