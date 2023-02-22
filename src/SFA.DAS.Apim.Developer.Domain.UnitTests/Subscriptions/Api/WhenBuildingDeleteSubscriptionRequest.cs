using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingDeleteSubscriptionRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Built_And_Model_Added_To_Request(
            string subscriptionId)
        {
            subscriptionId.Should().NotBeEmpty();

            subscriptionId += "+more things";
            var encodedSubscriptionId = HttpUtility.UrlEncode(subscriptionId);
            
            var actual = new DeleteSubscriptionRequest(subscriptionId);

            actual.DeleteUrl.Should().Be($"subscriptions/{encodedSubscriptionId}?api-version=2021-04-01-preview");
        }
    }
}